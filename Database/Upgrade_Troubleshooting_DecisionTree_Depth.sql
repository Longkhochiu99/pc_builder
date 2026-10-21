/*
    PCBuilderDB - upgrade the troubleshooting assistant to a multi-step decision tree.
    - Preserves the existing 30 result nodes and their edited Diagnosis/Action content.
    - Rebuilds the links so a user answers 4 question nodes in the selected group before a result.
    - Keeps navigation logic in C#; SQL stores questions, answers and NextNodeID edges.
    - Safe to re-run: if all groups already have intermediate question nodes, no changes are made.
    - Supports current schema dbo.TroubleshootingNodes / dbo.TroubleshootingOptions.

    BACKUP PCBuilderDB before running. Execute the entire file against PCBuilderDB in SSMS.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
USE PCBuilderDB;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @RootNodeID INT;
    IF (SELECT COUNT(*) FROM dbo.TroubleshootingNodes WHERE IsRoot = 1 AND NodeType = 'Q' AND IsActive = 1) <> 1
    BEGIN
        ;THROW 51001, N'Cây chẩn đoán cần đúng một node gốc đang hoạt động. Hãy kiểm tra TroubleshootingNodes.', 1;
    END;
    SELECT @RootNodeID = NodeID
    FROM dbo.TroubleshootingNodes
    WHERE IsRoot = 1 AND NodeType = 'Q' AND IsActive = 1;

    DECLARE @Groups TABLE
    (
        GroupKey CHAR(1) NOT NULL PRIMARY KEY,
        GroupNodeID INT NOT NULL UNIQUE,
        GroupLabel NVARCHAR(300) NOT NULL,
        RootSort SMALLINT NOT NULL
    );

    INSERT INTO @Groups (GroupKey, GroupNodeID, GroupLabel, RootSort)
    SELECT CASE
             WHEN rootOption.OptionText LIKE N'Nhóm A%' THEN 'A'
             WHEN rootOption.OptionText LIKE N'Nhóm B%' THEN 'B'
             WHEN rootOption.OptionText LIKE N'Nhóm C%' THEN 'C'
             WHEN rootOption.OptionText LIKE N'Nhóm D%' THEN 'D'
             WHEN rootOption.OptionText LIKE N'Nhóm E%' THEN 'E'
           END,
           category.NodeID,
           rootOption.OptionText,
           rootOption.SortOrder
    FROM dbo.TroubleshootingOptions AS rootOption
    JOIN dbo.TroubleshootingNodes AS category ON category.NodeID = rootOption.NextNodeID
    WHERE rootOption.NodeID = @RootNodeID
      AND category.NodeType = 'Q'
      AND (rootOption.OptionText LIKE N'Nhóm A%' OR rootOption.OptionText LIKE N'Nhóm B%'
        OR rootOption.OptionText LIKE N'Nhóm C%' OR rootOption.OptionText LIKE N'Nhóm D%'
        OR rootOption.OptionText LIKE N'Nhóm E%');

    IF (SELECT COUNT(*) FROM @Groups) <> 5
    BEGIN
        ;THROW 51002, N'Không tìm thấy đủ 5 nhóm A-E ở node gốc. Script chưa thay đổi dữ liệu.', 1;
    END;

    DECLARE @GroupsAlreadyDeep INT;
    SELECT @GroupsAlreadyDeep = COUNT(*)
    FROM @Groups AS g
    WHERE EXISTS
    (
        SELECT 1
        FROM dbo.TroubleshootingOptions AS e
        JOIN dbo.TroubleshootingNodes AS child ON child.NodeID = e.NextNodeID
        WHERE e.NodeID = g.GroupNodeID AND child.NodeType = 'Q'
    );

    IF @GroupsAlreadyDeep = 5
    BEGIN
        PRINT N'Cây quyết định nhiều tầng đã được cài đặt. Không cần cập nhật lại; dữ liệu admin được giữ nguyên.';
        COMMIT TRANSACTION;
        RETURN;
    END;

    IF @GroupsAlreadyDeep <> 0
    BEGIN
        ;THROW 51003, N'Cây đang ở trạng thái cập nhật dở dang (một số nhóm đã có node trung gian). Script dừng để tránh tạo nhánh trùng.', 1;
    END;

    DECLARE @LeafMap TABLE
    (
        GroupKey CHAR(1) NOT NULL,
        Slot INT NOT NULL,
        ResultNodeID INT NOT NULL,
        PRIMARY KEY (GroupKey, Slot)
    );

    ;WITH DirectResults AS
    (
        SELECT g.GroupKey,
               result.NodeID AS ResultNodeID,
               CONVERT(INT, edge.SortOrder) AS SortOrder,
               COUNT(*) OVER (PARTITION BY g.GroupKey) AS ResultCount,
               MAX(CONVERT(INT, edge.SortOrder)) OVER (PARTITION BY g.GroupKey) AS MaxSortOrder
        FROM @Groups AS g
        JOIN dbo.TroubleshootingOptions AS edge ON edge.NodeID = g.GroupNodeID
        JOIN dbo.TroubleshootingNodes AS result ON result.NodeID = edge.NextNodeID AND result.NodeType = 'R'
    ), ExistingResults AS
    (
        SELECT GroupKey, ResultNodeID,
               ROW_NUMBER() OVER
               (
                   PARTITION BY GroupKey
                   ORDER BY CASE WHEN ResultCount = 6 AND MaxSortOrder = 6 THEN SortOrder ELSE ResultNodeID END,
                            ResultNodeID
               ) AS Slot
        FROM DirectResults
    )
    INSERT INTO @LeafMap (GroupKey, Slot, ResultNodeID)
    SELECT GroupKey, CONVERT(INT, Slot), ResultNodeID
    FROM ExistingResults;

    IF EXISTS (SELECT g.GroupKey FROM @Groups AS g LEFT JOIN @LeafMap AS l ON l.GroupKey = g.GroupKey GROUP BY g.GroupKey HAVING COUNT(l.ResultNodeID) < 6)
    BEGIN
        ;THROW 51004, N'Mỗi nhóm cần có ít nhất 6 node kết luận trước khi chuyển sang cây nhiều tầng. Không có dữ liệu nào bị xóa.', 1;
    END;

    DECLARE @QuestionSeed TABLE
    (
        GroupKey CHAR(1) NOT NULL,
        NodeKey VARCHAR(10) NOT NULL,
        Question NVARCHAR(1000) NOT NULL,
        PRIMARY KEY (GroupKey, NodeKey)
    );

    DECLARE @EdgeSeed TABLE
    (
        GroupKey CHAR(1) NOT NULL,
        ParentKey VARCHAR(10) NOT NULL,
        OptionText NVARCHAR(300) NOT NULL,
        ChildKey VARCHAR(10) NOT NULL,
        SortOrder SMALLINT NOT NULL
    );

    INSERT INTO @QuestionSeed (GroupKey, NodeKey, Question) VALUES
    ('A', 'GROUP', N'Khi nhấn nút nguồn, biểu hiện nào gần nhất?'),
    ('A', 'Q2A', N'Sau khi nhấn nút nguồn, phản ứng về điện của máy ra sao?'),
    ('A', 'Q2B', N'Máy đã có điện nhưng quá trình khởi động bị dừng ở đâu?'),
    ('A', 'Q3A', N'Sau khi kiểm tra ổ cắm, dây nguồn và công tắc PSU, tình trạng nào xảy ra?'),
    ('A', 'Q3B', N'Khi máy bắt đầu khởi động, hành vi tiếp theo là gì?'),
    ('A', 'Q3C', N'Đèn debug hoặc tiếng bíp báo hiệu điều gì?'),
    ('A', 'Q3D', N'Máy đi được tới bước nào trước khi dừng?'),
    ('A', 'Q4A', N'Sau khi loại trừ nguồn điện bên ngoài, biểu hiện nào còn đúng?'),
    ('A', 'Q4B', N'Máy tắt trước POST hay bị dừng ở giai đoạn khởi tạo CPU?'),
    ('A', 'Q4C', N'Dấu hiệu POST nào xuất hiện rõ nhất?'),
    ('A', 'Q4D', N'Điểm dừng khởi động lặp lại là gì?'),
    ('B', 'GROUP', N'Triệu chứng RAM xuất hiện rõ nhất ở giai đoạn nào?'),
    ('B', 'Q2A', N'Khi vừa lắp/thay RAM hoặc đổi thiết lập BIOS, điều gì xảy ra?'),
    ('B', 'Q2B', N'Khi đã vào Windows, vấn đề thể hiện như thế nào?'),
    ('B', 'Q3A', N'Khi thử từng thanh RAM ở khe ưu tiên, máy phản ứng ra sao?'),
    ('B', 'Q3B', N'Lỗi có bắt đầu ngay sau khi bật XMP/EXPO không?'),
    ('B', 'Q3C', N'Lỗi ổn định xuất hiện khi chạy tác vụ nặng hay ngẫu nhiên?'),
    ('B', 'Q3D', N'Mức sử dụng bộ nhớ trong Task Manager đang ở trạng thái nào?'),
    ('B', 'Q4A', N'Sau khi tắt XMP/EXPO và trả BIOS về mặc định, tình trạng nào đúng?'),
    ('B', 'Q4B', N'Khi kiểm tra từng thanh/khe RAM, dấu hiệu nhận diện nào xuất hiện?'),
    ('B', 'Q4C', N'Khi theo dõi Windows, biểu hiện nổi bật nhất là gì?'),
    ('B', 'Q4D', N'Sau khi đóng ứng dụng ngốn bộ nhớ và kiểm tra lại, điều gì còn lại?'),
    ('C', 'GROUP', N'Biểu hiện liên quan CPU hoặc nhiệt độ rõ nhất là gì?'),
    ('C', 'Q2A', N'Nhiệt độ hay bộ làm mát có dấu hiệu bất thường nào?'),
    ('C', 'Q2B', N'Vấn đề xuất hiện chủ yếu dưới tải hay cả khi nhàn rỗi?'),
    ('C', 'Q3A', N'Khi máy nhàn rỗi, nhiệt độ CPU có cao bất thường không?'),
    ('C', 'Q3B', N'Quạt CPU hoặc bơm tản nhiệt có cảnh báo/tiếng lạ không?'),
    ('C', 'Q3C', N'Khi chơi game hoặc render, điều gì xảy ra với máy?'),
    ('C', 'Q3D', N'Trong Task Manager, xung và mức sử dụng CPU trông ra sao?'),
    ('C', 'Q4A', N'Kiểm tra quạt và nhiệt độ cho thấy tình trạng nào?'),
    ('C', 'Q4B', N'Dưới tải nặng, biểu hiện nào phản ánh vấn đề tản nhiệt?'),
    ('C', 'Q4C', N'Khi kiểm tra tiến trình và xung CPU, dấu hiệu nào đúng?'),
    ('C', 'Q4D', N'Hệ thống tản nhiệt cho biểu hiện nào sau khi chạy vài phút?'),
    ('D', 'GROUP', N'Vấn đề GPU/màn hình biểu hiện rõ nhất ở đâu?'),
    ('D', 'Q2A', N'Màn hình có nhận tín hiệu từ GPU rời không?'),
    ('D', 'Q2B', N'Khi đã có hình, lỗi thuộc loại hiển thị hay hiệu năng?'),
    ('D', 'Q3A', N'Sau khi kiểm tra cáp và cắm đúng cổng GPU, tình trạng nào?'),
    ('D', 'Q3B', N'Windows có nhận GPU trong Device Manager không?'),
    ('D', 'Q3C', N'Khi chơi game hoặc chạy GPU tải cao, điều gì xảy ra?'),
    ('D', 'Q3D', N'Sau khi cập nhật driver/thay đổi độ phân giải, lỗi nào xuất hiện?'),
    ('D', 'Q4A', N'Sau khi kiểm tra nguồn/cáp và GPU trong Windows, lựa chọn nào khớp nhất?'),
    ('D', 'Q4B', N'Vấn đề nổi bật hơn là sập hình dưới tải hay giảm hiệu năng?'),
    ('D', 'Q4C', N'Chất lượng tín hiệu hoặc thiết lập màn hình có biểu hiện nào?'),
    ('D', 'Q4D', N'Driver và hiệu năng sau cập nhật có trạng thái nào?'),
    ('E', 'GROUP', N'Thiết bị/SSD/mạng gặp sự cố theo cách nào?'),
    ('E', 'Q2A', N'Vấn đề tập trung ở thiết bị lưu trữ hay việc khởi động?'),
    ('E', 'Q2B', N'Sự cố xuất hiện ở kết nối mạng hay thiết bị ngoại vi?'),
    ('E', 'Q3A', N'Khi xem Task Manager/ổ đĩa, điều nào đáng chú ý?'),
    ('E', 'Q3B', N'BIOS và Windows hiện trạng thái nhận diện ổ SSD ra sao?'),
    ('E', 'Q3C', N'Thiết bị mạng kết nối nhưng vẫn lỗi Internet hay bị mất kết nối?'),
    ('E', 'Q3D', N'Thiết bị ngoại vi có xuất hiện trong Windows không?'),
    ('E', 'Q4A', N'Khi kiểm tra dung lượng và tình trạng ổ, dấu hiệu nào đúng?'),
    ('E', 'Q4B', N'Thiết bị nào không được nhận, và ở lớp nào?'),
    ('E', 'Q4C', N'Sau khi khởi động lại thiết bị/router và kiểm tra kết nối, dấu hiệu nào còn?'),
    ('E', 'Q4D', N'Lỗi rõ nhất nằm ở thiết bị ngoại vi cụ thể nào?');

    INSERT INTO @EdgeSeed (GroupKey, ParentKey, OptionText, ChildKey, SortOrder) VALUES
    ('A', 'GROUP', N'Hoàn toàn không lên nguồn hoặc nguồn chập chờn', 'Q2A', 1),
    ('A', 'GROUP', N'Có điện nhưng không hoàn tất POST/khởi động', 'Q2B', 2),
    ('A', 'Q2A', N'Không có đèn/quạt', 'Q3A', 1),
    ('A', 'Q2A', N'Có lúc lên rồi tắt hoặc phải bấm nhiều lần', 'Q3B', 2),
    ('A', 'Q2B', N'Đèn debug/bíp, không vào BIOS', 'Q3C', 1),
    ('A', 'Q2B', N'Dừng ở logo hãng hoặc quay lại BIOS', 'Q3D', 2),
    ('A', 'Q3A', N'Vẫn không có đèn/quạt sau khi thử nguồn ngoài', 'Q4A', 1),
    ('A', 'Q3A', N'Máy vừa nhận nguồn đã tắt hoặc reset', 'Q4B', 2),
    ('A', 'Q3B', N'Tắt sau vài giây hoặc lặp bật/tắt', 'Q4B', 1),
    ('A', 'Q3B', N'Có đèn debug hoặc tiếng bíp trước khi dừng', 'Q4C', 2),
    ('A', 'Q3C', N'Máy tắt trước khi vào BIOS', 'Q4B', 1),
    ('A', 'Q3C', N'CPU/DRAM/VGA/BOOT LED hoặc tiếng bíp báo lỗi', 'Q4C', 2),
    ('A', 'Q3D', N'Đèn debug hoặc tiếng bíp xuất hiện', 'Q4C', 1),
    ('A', 'Q3D', N'Dừng ở logo hoặc tự quay lại BIOS', 'Q4D', 2),
    ('A', 'Q4A', N'Vẫn hoàn toàn không có đèn hoặc quạt', 'R1', 1),
    ('A', 'Q4A', N'Nguồn chập chờn, phải nhấn nút nhiều lần', 'R5', 2),
    ('A', 'Q4B', N'Máy bật rồi tắt sau vài giây hoặc tự bật tắt', 'R2', 1),
    ('A', 'Q4B', N'Có điện nhưng đèn CPU sáng và không vào BIOS', 'R3', 2),
    ('A', 'Q4C', N'Đèn CPU/POST báo lỗi và không vào BIOS', 'R3', 1),
    ('A', 'Q4C', N'Có tiếng bíp hoặc đèn DRAM/VGA/BOOT sáng', 'R6', 2),
    ('A', 'Q4D', N'Dừng ở logo hãng hoặc quay lại BIOS', 'R4', 1),
    ('A', 'Q4D', N'Tiếng bíp hoặc đèn debug báo lỗi khởi động', 'R6', 2),
    ('B', 'GROUP', N'Không boot/không nhận đủ RAM sau khi lắp hoặc thay đổi thiết lập', 'Q2A', 1),
    ('B', 'GROUP', N'Windows vào được nhưng máy không ổn định hoặc chậm', 'Q2B', 2),
    ('B', 'Q2A', N'Không lên hình hoặc không nhận đủ thanh RAM', 'Q3A', 1),
    ('B', 'Q2A', N'Lỗi bắt đầu sau khi bật XMP/EXPO', 'Q3B', 2),
    ('B', 'Q2B', N'Treo, BSOD hoặc ứng dụng tự đóng', 'Q3C', 1),
    ('B', 'Q2B', N'RAM gần đầy và máy chậm khi mở nhiều ứng dụng', 'Q3D', 2),
    ('B', 'Q3A', N'Một thanh/khe không được nhận', 'Q4B', 1),
    ('B', 'Q3A', N'Có nhận RAM nhưng phát sinh BSOD sau thay đổi', 'Q4A', 2),
    ('B', 'Q3B', N'Tắt XMP/EXPO thì máy ổn định hơn', 'Q4A', 1),
    ('B', 'Q3B', N'Vẫn không nhận thanh hoặc kênh RAM', 'Q4B', 2),
    ('B', 'Q3C', N'Treo/văng ứng dụng khi đa nhiệm, không rõ liên quan cấu hình RAM', 'Q4C', 1),
    ('B', 'Q3C', N'Lỗi bắt đầu ngay sau khi nâng cấp/ép xung RAM', 'Q4A', 2),
    ('B', 'Q3D', N'RAM gần đầy và có tiến trình dùng nhiều bộ nhớ', 'Q4C', 1),
    ('B', 'Q3D', N'RAM không đầy nhưng máy vẫn treo/đóng ứng dụng', 'Q4D', 2),
    ('B', 'Q4A', N'Màn hình xanh hoặc khởi động lại sau khi nâng cấp RAM', 'R1', 1),
    ('B', 'Q4A', N'Bật XMP/EXPO xong máy không boot hoặc báo lỗi bộ nhớ', 'R5', 2),
    ('B', 'Q4B', N'Không lên hình sau khi lắp thêm hoặc thay RAM', 'R2', 1),
    ('B', 'Q4B', N'Chỉ nhận một thanh RAM dù đang lắp hai thanh', 'R3', 2),
    ('B', 'Q4C', N'Máy treo hoặc ứng dụng tự đóng khi chạy nhiều tác vụ', 'R4', 1),
    ('B', 'Q4C', N'RAM thường xuyên gần đầy và máy chậm khi mở nhiều ứng dụng', 'R6', 2),
    ('B', 'Q4D', N'Treo hoặc đóng ứng dụng ngay cả khi bộ nhớ chưa đầy', 'R4', 1),
    ('B', 'Q4D', N'RAM thường xuyên gần đầy khiến máy chậm khi đa nhiệm', 'R6', 2),
    ('C', 'GROUP', N'Nhiệt độ cao hoặc tản nhiệt có vẻ bất thường', 'Q2A', 1),
    ('C', 'GROUP', N'Hiệu năng CPU không ổn định, chậm hoặc tụt xung', 'Q2B', 2),
    ('C', 'Q2A', N'Nóng ngay cả khi ít tải', 'Q3A', 1),
    ('C', 'Q2A', N'Quạt báo lỗi hoặc tản nhiệt phát tiếng lạ', 'Q3B', 2),
    ('C', 'Q2B', N'Tắt/tụt hiệu năng khi chơi game hoặc render', 'Q3C', 1),
    ('C', 'Q2B', N'CPU dùng gần 100% hoặc xung thấp', 'Q3D', 2),
    ('C', 'Q3A', N'Nhiệt độ cao khi nhàn rỗi hoặc quạt có cảnh báo', 'Q4A', 1),
    ('C', 'Q3A', N'Nhiệt tăng mạnh khi tải hoặc bơm có tiếng lạ', 'Q4B', 2),
    ('C', 'Q3B', N'CPU_FAN/pump không chạy hoặc báo lỗi', 'Q4A', 1),
    ('C', 'Q3B', N'Bơm phát tiếng lạ và nhiệt độ tăng nhanh', 'Q4B', 2),
    ('C', 'Q3C', N'Nhiệt độ tăng mạnh hoặc máy tự tắt', 'Q4B', 1),
    ('C', 'Q3C', N'Nhiệt không quá cao nhưng xung CPU giảm', 'Q4D', 2),
    ('C', 'Q3D', N'Một tiến trình chiếm CPU gần 100%', 'Q4C', 1),
    ('C', 'Q3D', N'CPU chạy xung thấp dù không quá nóng', 'Q4D', 2),
    ('C', 'Q4A', N'Nhiệt độ CPU cao bất thường ngay cả khi ít tải', 'R1', 1),
    ('C', 'Q4A', N'BIOS báo CPU Fan Error hoặc quạt CPU không quay', 'R3', 2),
    ('C', 'Q4B', N'Máy tắt đột ngột hoặc tụt hiệu năng khi chơi game/render', 'R2', 1),
    ('C', 'Q4B', N'Tản nhiệt nước phát tiếng lạ hoặc nhiệt độ CPU tăng nhanh', 'R6', 2),
    ('C', 'Q4C', N'CPU luôn sử dụng gần 100% dù không chạy tác vụ nặng', 'R4', 1),
    ('C', 'Q4C', N'CPU chạy ở xung thấp và máy chậm hơn bình thường', 'R5', 2),
    ('C', 'Q4D', N'CPU nóng bất thường khi kiểm tra nhiệt độ', 'R1', 1),
    ('C', 'Q4D', N'Nhiệt độ CPU tăng nhanh kèm tiếng lạ từ bơm tản nhiệt', 'R6', 2),
    ('D', 'GROUP', N'Không có tín hiệu hoặc GPU không được nhận', 'Q2A', 1),
    ('D', 'GROUP', N'Có hình nhưng hiển thị/hiệu năng bất thường', 'Q2B', 2),
    ('D', 'Q2A', N'No Signal/không lên hình', 'Q3A', 1),
    ('D', 'Q2A', N'Không cài được driver hoặc Code 43', 'Q3B', 2),
    ('D', 'Q2B', N'Màn hình đen hoặc game văng dưới tải', 'Q3C', 1),
    ('D', 'Q2B', N'Nhấp nháy/sọc/FPS/độ phân giải bất thường', 'Q3D', 2),
    ('D', 'Q3A', N'Đã đổi cáp/cổng mà màn hình vẫn không có tín hiệu', 'Q4A', 1),
    ('D', 'Q3A', N'Hình lên được nhưng game văng dưới tải', 'Q4B', 2),
    ('D', 'Q3B', N'Device Manager báo Code 43/driver không cài được', 'Q4A', 1),
    ('D', 'Q3B', N'GPU được nhận nhưng màn hình có sọc/nhấp nháy', 'Q4C', 2),
    ('D', 'Q3C', N'Màn hình đen hoặc game văng dưới tải', 'Q4B', 1),
    ('D', 'Q3C', N'FPS giảm rõ rệt sau cập nhật driver', 'Q4D', 2),
    ('D', 'Q3D', N'Sọc/nhấp nháy/tần số quét sai', 'Q4C', 1),
    ('D', 'Q3D', N'Hiệu năng giảm hoặc driver báo lỗi', 'Q4D', 2),
    ('D', 'Q4A', N'Màn hình báo No Signal khi dùng card đồ họa rời', 'R1', 1),
    ('D', 'Q4A', N'Không cài được driver GPU hoặc Device Manager báo Code 43', 'R5', 2),
    ('D', 'Q4B', N'Màn hình đen hoặc game văng khi GPU chịu tải nặng', 'R2', 1),
    ('D', 'Q4B', N'FPS giảm rõ rệt sau khi cập nhật driver đồ họa', 'R4', 2),
    ('D', 'Q4C', N'Màn hình nhấp nháy, xuất hiện sọc hoặc điểm ảnh bất thường', 'R3', 1),
    ('D', 'Q4C', N'Độ phân giải hoặc tần số quét không đạt mức mong muốn', 'R6', 2),
    ('D', 'Q4D', N'FPS giảm rõ sau khi cập nhật driver', 'R4', 1),
    ('D', 'Q4D', N'Driver GPU lỗi hoặc Device Manager báo Code 43', 'R5', 2),
    ('E', 'GROUP', N'SSD/khởi động/ổ đĩa chậm hoặc không được nhận', 'Q2A', 1),
    ('E', 'GROUP', N'Mạng hoặc thiết bị ngoại vi không hoạt động', 'Q2B', 2),
    ('E', 'Q2A', N'Windows chậm/ổ đĩa sử dụng 100%', 'Q3A', 1),
    ('E', 'Q2A', N'SSD biến mất hoặc không khởi động được', 'Q3B', 2),
    ('E', 'Q2B', N'Mạng kết nối nhưng không vào Internet', 'Q3C', 1),
    ('E', 'Q2B', N'USB/Bluetooth/máy in/âm thanh có vấn đề', 'Q3D', 2),
    ('E', 'Q3A', N'Ổ gần đầy hoặc có lỗi đọc ghi', 'Q4A', 1),
    ('E', 'Q3A', N'Ổ đĩa 100%/khởi động chậm nhưng vẫn được nhận', 'Q4B', 2),
    ('E', 'Q3B', N'SSD không xuất hiện trong BIOS/Windows', 'Q4B', 1),
    ('E', 'Q3B', N'SSD được nhận nhưng gần đầy hoặc lỗi đọc ghi', 'Q4A', 2),
    ('E', 'Q3C', N'Wi-Fi/Ethernet báo kết nối nhưng không có Internet', 'Q4C', 1),
    ('E', 'Q3C', N'Lỗi xuất hiện khi ghép nối Bluetooth/máy in/âm thanh', 'Q4D', 2),
    ('E', 'Q3D', N'Bàn phím/chuột/USB không được Windows nhận', 'Q4D', 1),
    ('E', 'Q3D', N'Thiết bị được nhận nhưng mất kết nối/không hoạt động', 'Q4C', 2),
    ('E', 'Q4A', N'Windows khởi động chậm hoặc ổ đĩa thường xuyên 100%', 'R1', 1),
    ('E', 'Q4A', N'Ổ SSD gần đầy hoặc xuất hiện lỗi đọc ghi', 'R3', 2),
    ('E', 'Q4B', N'SSD không xuất hiện trong BIOS hoặc Windows', 'R2', 1),
    ('E', 'Q4B', N'Bàn phím, chuột hoặc thiết bị USB không được nhận', 'R5', 2),
    ('E', 'Q4C', N'Wi-Fi hoặc Ethernet kết nối nhưng không vào Internet', 'R4', 1),
    ('E', 'Q4C', N'Bluetooth, máy in hoặc thiết bị âm thanh ngoại vi không hoạt động', 'R6', 2),
    ('E', 'Q4D', N'Bàn phím, chuột hoặc thiết bị USB không được nhận', 'R5', 1),
    ('E', 'Q4D', N'Bluetooth, máy in hoặc thiết bị âm thanh ngoại vi không hoạt động', 'R6', 2);

    DECLARE @NodeMap TABLE
    (
        GroupKey CHAR(1) NOT NULL,
        NodeKey VARCHAR(10) NOT NULL,
        NodeID INT NOT NULL,
        PRIMARY KEY (GroupKey, NodeKey)
    );

    INSERT INTO @NodeMap (GroupKey, NodeKey, NodeID)
    SELECT GroupKey, 'GROUP', GroupNodeID FROM @Groups;

    UPDATE node
    SET Question = seed.Question
    FROM dbo.TroubleshootingNodes AS node
    JOIN @Groups AS g ON g.GroupNodeID = node.NodeID
    JOIN @QuestionSeed AS seed ON seed.GroupKey = g.GroupKey AND seed.NodeKey = 'GROUP';

    DECLARE @GroupKey CHAR(1), @NodeKey VARCHAR(10), @Question NVARCHAR(1000), @NewNodeID INT;
    DECLARE question_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT GroupKey, NodeKey, Question
        FROM @QuestionSeed
        WHERE NodeKey <> 'GROUP'
        ORDER BY GroupKey, NodeKey;

    OPEN question_cursor;
    FETCH NEXT FROM question_cursor INTO @GroupKey, @NodeKey, @Question;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        INSERT INTO dbo.TroubleshootingNodes (NodeType, Question, Diagnosis, [Action], IsRoot, IsActive)
        VALUES ('Q', @Question, NULL, NULL, 0, 1);
        SET @NewNodeID = CONVERT(INT, SCOPE_IDENTITY());
        INSERT INTO @NodeMap (GroupKey, NodeKey, NodeID) VALUES (@GroupKey, @NodeKey, @NewNodeID);
        FETCH NEXT FROM question_cursor INTO @GroupKey, @NodeKey, @Question;
    END;
    CLOSE question_cursor;
    DEALLOCATE question_cursor;

    /* Remove the old direct group -> result shortcuts, but keep each result node and its content. */
    DELETE edge
    FROM dbo.TroubleshootingOptions AS edge
    JOIN @Groups AS g ON g.GroupNodeID = edge.NodeID;

    /* Insert group -> Q2 -> Q3 -> Q4 -> result links. Every result is reached only after 4 questions. */
    INSERT INTO dbo.TroubleshootingOptions (NodeID, OptionText, NextNodeID, SortOrder)
    SELECT parent.NodeID,
           edge.OptionText,
           COALESCE(childQuestion.NodeID, childResult.ResultNodeID),
           edge.SortOrder
    FROM @EdgeSeed AS edge
    JOIN @NodeMap AS parent ON parent.GroupKey = edge.GroupKey AND parent.NodeKey = edge.ParentKey
    LEFT JOIN @NodeMap AS childQuestion ON childQuestion.GroupKey = edge.GroupKey AND childQuestion.NodeKey = edge.ChildKey
    LEFT JOIN @LeafMap AS childResult ON childResult.GroupKey = edge.GroupKey
         AND edge.ChildKey LIKE 'R%'
         AND childResult.Slot = TRY_CONVERT(INT, SUBSTRING(edge.ChildKey, 2, 9))
    WHERE COALESCE(childQuestion.NodeID, childResult.ResultNodeID) IS NOT NULL;

    /* Any administrator-added result beyond the original six is preserved as an extra final answer. */
    ;WITH ExtraResults AS
    (
        SELECT GroupKey, Slot, ResultNodeID,
               ROW_NUMBER() OVER (PARTITION BY GroupKey ORDER BY Slot) AS ExtraOrder
        FROM @LeafMap
        WHERE Slot > 6
    )
    INSERT INTO dbo.TroubleshootingOptions (NodeID, OptionText, NextNodeID, SortOrder)
    SELECT q4.NodeID,
           LEFT(result.Question, 300),
           result.NodeID,
           CONVERT(SMALLINT, 2 + extra.ExtraOrder)
    FROM ExtraResults AS extra
    JOIN @NodeMap AS q4 ON q4.GroupKey = extra.GroupKey AND q4.NodeKey = 'Q4A'
    JOIN dbo.TroubleshootingNodes AS result ON result.NodeID = extra.ResultNodeID;

    COMMIT TRANSACTION;

    PRINT N'Đã nâng cấp cây quyết định: mỗi nhánh hỏi qua 4 node câu hỏi trước khi đưa ra kết luận.';
    PRINT N'Đã giữ nguyên nội dung 30 kết luận hiện có (Question/Diagnosis/Action), đồng thời giữ lại các kết luận admin đã thêm nếu có.';
    PRINT N'Form quản lý sự cố cần dùng bản code mới để tìm kiếm/chuyển kết luận theo toàn bộ nhánh cây.';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorNumber INT = ERROR_NUMBER();
    RAISERROR(N'Nâng cấp cây quyết định thất bại (%d): %s', 16, 1, @ErrorNumber, @ErrorMessage);
END CATCH;
GO
