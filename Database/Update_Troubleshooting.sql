/*
    PCBuilderDB - upgrade the troubleshooting decision tree
    - Creates/updates only dbo.TroubleshootingNodes and dbo.TroubleshootingOptions.
    - Uses the schema requested for the application.
    - Seeds 5 groups x 6 issues only when the troubleshooting tree is empty.
    - Re-running this script after admin edits does not overwrite the edited data.

    Run this file in SQL Server Management Studio against the server that hosts PCBuilderDB.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

USE PCBuilderDB;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    /* One-time conversion from the old schema (NodeCode/Title/QuestionText and
       NodeType='Question'/'Diagnosis') to the requested schema. Only these two
       troubleshooting tables are recreated; the rest of the database is untouched. */
    IF OBJECT_ID(N'dbo.TroubleshootingNodes', N'U') IS NOT NULL
       AND (
            COL_LENGTH(N'dbo.TroubleshootingNodes', N'Question') IS NULL OR
            COL_LENGTH(N'dbo.TroubleshootingNodes', N'Action') IS NULL OR
            COL_LENGTH(N'dbo.TroubleshootingNodes', N'IsRoot') IS NULL
       )
    BEGIN
        IF OBJECT_ID(N'dbo.TroubleshootingOptions', N'U') IS NOT NULL
            DROP TABLE dbo.TroubleshootingOptions;
        DROP TABLE dbo.TroubleshootingNodes;
    END;

    IF OBJECT_ID(N'dbo.TroubleshootingOptions', N'U') IS NOT NULL
       AND COL_LENGTH(N'dbo.TroubleshootingOptions', N'OptionText') IS NULL
    BEGIN
        DROP TABLE dbo.TroubleshootingOptions;
    END;

    IF OBJECT_ID(N'dbo.TroubleshootingNodes', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.TroubleshootingNodes
        (
            NodeID      INT IDENTITY(1,1) NOT NULL,
            NodeType    CHAR(1) NOT NULL,             -- Q = question, R = result
            Question    NVARCHAR(1000) NOT NULL,       -- question text or issue title
            Diagnosis   NVARCHAR(2000) NULL,
            [Action]    NVARCHAR(4000) NULL,
            IsRoot      BIT NOT NULL CONSTRAINT DF_TroubleshootingNodes_IsRoot DEFAULT (0),
            IsActive    BIT NOT NULL CONSTRAINT DF_TroubleshootingNodes_IsActive DEFAULT (1),
            CONSTRAINT PK_TroubleshootingNodes PRIMARY KEY (NodeID),
            CONSTRAINT CK_TroubleshootingNodes_NodeType CHECK (NodeType IN ('Q', 'R')),
            CONSTRAINT CK_TroubleshootingNodes_RootType CHECK (IsRoot = 0 OR NodeType = 'Q'),
            CONSTRAINT CK_TroubleshootingNodes_ResultContent CHECK
                (NodeType <> 'R' OR (Diagnosis IS NOT NULL AND [Action] IS NOT NULL))
        );
    END;

    IF OBJECT_ID(N'dbo.TroubleshootingOptions', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.TroubleshootingOptions
        (
            OptionID     INT IDENTITY(1,1) NOT NULL,
            NodeID       INT NOT NULL,
            OptionText   NVARCHAR(300) NOT NULL,
            NextNodeID   INT NOT NULL,
            SortOrder    SMALLINT NOT NULL CONSTRAINT DF_TroubleshootingOptions_SortOrder DEFAULT (1),
            CONSTRAINT PK_TroubleshootingOptions PRIMARY KEY (OptionID),
            CONSTRAINT FK_TroubleshootingOptions_Node FOREIGN KEY (NodeID)
                REFERENCES dbo.TroubleshootingNodes(NodeID) ON DELETE CASCADE,
            CONSTRAINT FK_TroubleshootingOptions_NextNode FOREIGN KEY (NextNodeID)
                REFERENCES dbo.TroubleshootingNodes(NodeID),
            CONSTRAINT CK_TroubleshootingOptions_SortOrder CHECK (SortOrder > 0),
            CONSTRAINT UQ_TroubleshootingOptions_Node_Order UNIQUE (NodeID, SortOrder)
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.TroubleshootingNodes)
    BEGIN
        DECLARE @RootNodeID INT;
        DECLARE @GroupA INT, @GroupB INT, @GroupC INT, @GroupD INT, @GroupE INT;

        INSERT INTO dbo.TroubleshootingNodes (NodeType, Question, Diagnosis, [Action], IsRoot, IsActive)
        VALUES ('Q', N'Bạn đang gặp sự cố thuộc nhóm nào?', NULL, NULL, 1, 1);
        SET @RootNodeID = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.TroubleshootingNodes (NodeType, Question, Diagnosis, [Action], IsRoot, IsActive)
        VALUES ('Q', N'Nhóm A — Nguồn điện và khởi động: hãy chọn lỗi hoặc triệu chứng gần nhất.', NULL, NULL, 0, 1);
        SET @GroupA = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.TroubleshootingNodes (NodeType, Question, Diagnosis, [Action], IsRoot, IsActive)
        VALUES ('Q', N'Nhóm B — RAM và độ ổn định hệ thống: hãy chọn lỗi hoặc triệu chứng gần nhất.', NULL, NULL, 0, 1);
        SET @GroupB = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.TroubleshootingNodes (NodeType, Question, Diagnosis, [Action], IsRoot, IsActive)
        VALUES ('Q', N'Nhóm C — CPU và nhiệt độ: hãy chọn lỗi hoặc triệu chứng gần nhất.', NULL, NULL, 0, 1);
        SET @GroupC = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.TroubleshootingNodes (NodeType, Question, Diagnosis, [Action], IsRoot, IsActive)
        VALUES ('Q', N'Nhóm D — GPU, màn hình và driver: hãy chọn lỗi hoặc triệu chứng gần nhất.', NULL, NULL, 0, 1);
        SET @GroupD = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.TroubleshootingNodes (NodeType, Question, Diagnosis, [Action], IsRoot, IsActive)
        VALUES ('Q', N'Nhóm E — SSD, mạng và thiết bị ngoại vi: hãy chọn lỗi hoặc triệu chứng gần nhất.', NULL, NULL, 0, 1);
        SET @GroupE = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.TroubleshootingOptions (NodeID, OptionText, NextNodeID, SortOrder)
        VALUES
            (@RootNodeID, N'Nhóm A — Nguồn điện và khởi động', @GroupA, 1),
            (@RootNodeID, N'Nhóm B — RAM và độ ổn định hệ thống', @GroupB, 2),
            (@RootNodeID, N'Nhóm C — CPU và nhiệt độ', @GroupC, 3),
            (@RootNodeID, N'Nhóm D — GPU, màn hình và driver', @GroupD, 4),
            (@RootNodeID, N'Nhóm E — SSD, mạng và thiết bị ngoại vi', @GroupE, 5);

        DECLARE @Issues TABLE
        (
            GroupNodeID INT NOT NULL,
            SortOrder SMALLINT NOT NULL,
            Question NVARCHAR(1000) NOT NULL,
            Diagnosis NVARCHAR(2000) NOT NULL,
            [Action] NVARCHAR(4000) NOT NULL
        );

        INSERT INTO @Issues (GroupNodeID, SortOrder, Question, Diagnosis, [Action])
        VALUES
        /* Nhóm A — Nguồn điện và khởi động */
        (@GroupA, 1, N'Bấm nút nguồn nhưng máy hoàn toàn không có đèn hoặc quạt',
         N'Khả năng cao máy chưa nhận điện, PSU đang tắt/bảo vệ hoặc dây cấp nguồn bị lỏng.',
         N'Kiểm tra ổ cắm bằng thiết bị khác; cắm chặt dây nguồn; kiểm tra công tắc I/O trên PSU và đầu 24-pin mainboard/8-pin CPU khi đã rút điện. Không tự mở vỏ PSU. Nếu vẫn không có dấu hiệu cấp điện, nhờ kỹ thuật viên kiểm tra PSU và mainboard.'),
        (@GroupA, 2, N'Máy bật lên rồi tắt sau vài giây hoặc tự bật tắt liên tục',
         N'Có thể PSU kích hoạt bảo vệ, có chập điện, dây CPU chưa cắm đúng hoặc tản nhiệt CPU lắp chưa chuẩn.',
         N'Rút điện, tháo thiết bị ngoại vi không cần thiết và kiểm tra đầu 24-pin/CPU 8-pin. Quan sát xem có mùi khét hoặc dấu hiệu chạm vỏ không; không thử bật lặp đi lặp lại. Nếu mới lắp ráp, kiểm tra chân đế mainboard và nhờ kỹ thuật viên kiểm tra PSU/đoản mạch.'),
        (@GroupA, 3, N'Máy có điện nhưng đèn CPU sáng và không vào BIOS',
         N'Quá trình POST có thể đang dừng ở CPU, nguồn CPU, BIOS hoặc socket CPU.',
         N'Tắt máy và ngắt điện trước khi thao tác. Kiểm tra dây nguồn CPU 8-pin, xem mã đèn debug trong sách hướng dẫn mainboard. Xác minh CPU có được BIOS hỗ trợ; chỉ tháo/lắp CPU khi có kinh nghiệm vì chân socket rất dễ hỏng.'),
        (@GroupA, 4, N'Máy dừng ở logo hãng hoặc tự quay lại BIOS khi khởi động',
         N'Có thể thứ tự boot sai, thiết bị USB gây cản trở hoặc ổ hệ điều hành không được nhận.',
         N'Thử rút các USB không cần thiết, kiểm tra BIOS có nhận SSD/HDD hay không và đặt đúng Windows Boot Manager làm thiết bị khởi động. Nếu ổ đĩa chập chờn, ưu tiên sao lưu dữ liệu trước khi sửa chữa.'),
        (@GroupA, 5, N'Phải nhấn nút nguồn nhiều lần hoặc rút cắm dây điện mới bật được',
         N'Có thể nút nguồn/cáp front-panel tiếp xúc kém hoặc PSU/mainboard hoạt động không ổn định.',
         N'Kiểm tra ổ cắm và dây nguồn khác phù hợp; kiểm tra giắc PWR_SW theo sơ đồ mainboard khi đã rút điện. Không chập chân trực tiếp nếu không rõ thao tác. Nếu lỗi tái diễn, kiểm tra PSU và mainboard.'),
        (@GroupA, 6, N'Máy phát tiếng bíp hoặc đèn debug DRAM/VGA/BOOT sáng',
         N'Mã bíp hoặc đèn debug đang chỉ tới bước POST gặp trục trặc; ý nghĩa phụ thuộc hãng mainboard.',
         N'Ghi lại mã bíp/đèn đang sáng và tra sách hướng dẫn mainboard. Rút điện trước khi lắp lại RAM/GPU và kiểm tra dây nguồn tương ứng. Thử cấu hình tối thiểu; không tháo linh kiện khi máy đang có điện.'),

        /* Nhóm B — RAM và độ ổn định hệ thống */
        (@GroupB, 1, N'Máy bị màn hình xanh hoặc khởi động lại ngẫu nhiên sau khi nâng cấp RAM',
         N'RAM mới có thể chưa tương thích, cấu hình XMP/EXPO quá cao hoặc bộ nhớ không ổn định.',
         N'Tắt XMP/EXPO trong BIOS để thử lại mặc định; kiểm tra RAM có nằm trong cấu hình hỗ trợ của mainboard/CPU. Chạy Windows Memory Diagnostic hoặc MemTest86 theo hướng dẫn, thử từng thanh riêng và sao lưu dữ liệu quan trọng.'),
        (@GroupB, 2, N'Máy không lên hình sau khi lắp thêm hoặc thay RAM',
         N'Thanh RAM có thể chưa cắm hết, lắp sai khe ưu tiên hoặc bộ nhớ chưa được huấn luyện ổn định.',
         N'Ngắt điện, lắp lại RAM cho đến khi chốt khóa cố định; kiểm tra vị trí khe khuyến nghị trong sách mainboard. Khởi động với một thanh ở khe ưu tiên và chờ quá trình training hoàn tất trước khi kết luận lỗi.'),
        (@GroupB, 3, N'Máy chỉ nhận một thanh RAM dù đang lắp hai thanh',
         N'Một thanh RAM, khe RAM hoặc kênh bộ nhớ của CPU/socket có thể gặp vấn đề.',
         N'Tắt điện và thử từng thanh một, sau đó thử các khe theo hướng dẫn mainboard. Tránh chạm vào chân tiếp xúc; nếu kết quả phụ thuộc khe, kiểm tra socket CPU/mainboard tại trung tâm kỹ thuật.'),
        (@GroupB, 4, N'Máy treo hoặc ứng dụng tự đóng khi chạy nhiều tác vụ',
         N'Có thể RAM không ổn định, hệ thống quá tải bộ nhớ hoặc ứng dụng/driver gây lỗi.',
         N'Kiểm tra mức sử dụng RAM trong Task Manager, đóng ứng dụng ngốn bộ nhớ và cập nhật ứng dụng/driver. Chạy kiểm tra bộ nhớ; nếu lỗi bắt đầu sau khi ép xung RAM, trả BIOS về mặc định để đối chiếu.'),
        (@GroupB, 5, N'Bật XMP/EXPO xong máy không boot hoặc báo lỗi bộ nhớ',
         N'Tốc độ/timing đang chọn có thể vượt mức ổn định của CPU, mainboard hoặc bộ kit RAM.',
         N'Khôi phục BIOS về mặc định hoặc Clear CMOS theo sách mainboard; xác nhận máy chạy ổn định ở tốc độ mặc định trước. Nếu cần, thử profile thấp hơn và cập nhật BIOS ổn định từ trang chính hãng.'),
        (@GroupB, 6, N'RAM thường xuyên gần đầy và máy chậm khi mở nhiều ứng dụng',
         N'Có thể thiếu dung lượng RAM cho khối lượng công việc hoặc có tiến trình chiếm bộ nhớ bất thường.',
         N'Trong Task Manager, sắp xếp theo Memory để xác định ứng dụng chiếm nhiều RAM; tắt ứng dụng khởi động không cần thiết và quét bảo mật nếu có tiến trình lạ. Chỉ cân nhắc nâng cấp RAM sau khi xác nhận nhu cầu thực tế.'),

        /* Nhóm C — CPU và nhiệt độ */
        (@GroupC, 1, N'Nhiệt độ CPU cao bất thường ngay cả khi ít tải',
         N'Tản nhiệt có thể tiếp xúc kém, quạt/pump không chạy hoặc luồng gió trong case bị cản.',
         N'Dùng phần mềm uy tín theo dõi nhiệt độ và tốc độ quạt; kiểm tra quạt CPU/pump có quay và dây được cắm đúng đầu. Khi lắp lại tản nhiệt, kiểm tra lớp keo tản nhiệt và màng bảo vệ; nếu không quen thao tác, nhờ kỹ thuật viên.'),
        (@GroupC, 2, N'Máy tắt đột ngột hoặc tụt hiệu năng khi chơi game/render',
         N'CPU có thể quá nhiệt, bị giới hạn công suất hoặc hệ thống làm mát không đáp ứng tải.',
         N'Vệ sinh bụi ở lọc bụi/tản nhiệt, đảm bảo quạt hút-xả hoạt động và theo dõi nhiệt độ khi tải. Trả thiết lập ép xung về mặc định; nếu nhiệt độ vẫn chạm giới hạn hoặc máy tiếp tục tắt, dừng thử tải và kiểm tra tản nhiệt/PSU.'),
        (@GroupC, 3, N'BIOS báo CPU Fan Error hoặc quạt CPU không quay',
         N'Quạt có thể chưa cắm vào CPU_FAN, đường dây hỏng hoặc quạt/pump gặp sự cố.',
         N'Kiểm tra đầu quạt được cắm vào CPU_FAN theo hướng dẫn mainboard và quan sát dây có bị kẹt không. Không vô hiệu cảnh báo khi chưa xác nhận tản nhiệt hoạt động. Nếu quạt/pump không chạy, thay hoặc sửa trước khi tiếp tục dùng máy.'),
        (@GroupC, 4, N'CPU luôn sử dụng gần 100% dù không chạy tác vụ nặng',
         N'Một ứng dụng nền, bản cập nhật, tiến trình lỗi hoặc phần mềm không mong muốn có thể chiếm CPU.',
         N'Mở Task Manager, sắp xếp theo CPU và xác định tiến trình đứng đầu; kiểm tra Windows Update và ứng dụng khởi động. Quét bằng công cụ bảo mật tin cậy; không kết thúc tiến trình hệ thống khi chưa biết rõ chức năng.'),
        (@GroupC, 5, N'CPU chạy ở xung thấp và máy chậm hơn bình thường',
         N'CPU có thể bị giới hạn bởi nhiệt độ, chế độ điện năng, BIOS hoặc giới hạn công suất.',
         N'Kiểm tra nhiệt độ, chế độ Power mode và thiết lập BIOS; hoàn tác ép xung/undervolt nếu vừa thay đổi. Đối chiếu xung khi có tải thực tế và tránh ép CPU vượt thông số nhà sản xuất.'),
        (@GroupC, 6, N'Tản nhiệt nước phát tiếng lạ hoặc nhiệt độ CPU tăng nhanh',
         N'Bơm có thể không hoạt động, có khí trong vòng nước hoặc bộ tản nhiệt bị tắc/bám bụi.',
         N'Kiểm tra nguồn bơm, đầu cắm AIO_PUMP/CPU_FAN và tốc độ bơm theo tài liệu của hãng. Dừng tải nặng nếu nhiệt độ tăng nhanh; không tháo mở mạch nước kín khi chưa có chuyên môn và liên hệ bảo hành khi nghi bơm hỏng.'),

        /* Nhóm D — GPU, màn hình và driver */
        (@GroupD, 1, N'Màn hình báo No Signal khi dùng card đồ họa rời',
         N'Cáp có thể cắm nhầm cổng, GPU chưa gắn chặt hoặc thiếu nguồn PCIe.',
         N'Chọn đúng input trên màn hình; cắm cáp vào cổng trên GPU rời và thử dây/cổng khác. Tắt máy, ngắt điện rồi kiểm tra GPU đã vào hết khe PCIe và các đầu nguồn GPU được gắn chắc.'),
        (@GroupD, 2, N'Màn hình đen hoặc game văng khi GPU chịu tải nặng',
         N'Có thể driver bị lỗi, GPU quá nhiệt hoặc PSU/đầu nguồn không ổn định.',
         N'Kiểm tra nhiệt độ GPU, đầu cấp nguồn và log lỗi; trả ép xung GPU về mặc định. Cài lại driver chính hãng sạch sẽ và thử tải nhẹ hơn. Nếu có mùi khét, tiếng bất thường hoặc tắt nguồn lặp lại, dừng sử dụng và kiểm tra phần cứng.'),
        (@GroupD, 3, N'Màn hình nhấp nháy, xuất hiện sọc hoặc điểm ảnh bất thường',
         N'Nguyên nhân có thể là cáp/đầu nối, tần số quét, driver hoặc lỗi phần cứng màn hình/GPU.',
         N'Thử cáp khác đạt chuẩn, cắm lại đầu nối và đặt tần số quét được màn hình hỗ trợ. Chụp ảnh màn hình để phân biệt lỗi nội dung với panel/cáp; nếu sọc xuất hiện cả trong BIOS hoặc trên nhiều màn hình, cần kiểm tra phần cứng.'),
        (@GroupD, 4, N'FPS giảm rõ rệt sau khi cập nhật driver đồ họa',
         N'Phiên bản driver mới có thể xung đột, cài đặt bị đặt lại hoặc trò chơi đang dùng GPU không mong muốn.',
         N'Khởi động lại máy, xác nhận trò chơi sử dụng GPU rời và kiểm tra mức tải/nhiệt độ. Nếu lỗi bắt đầu ngay sau cập nhật, cài lại hoặc quay về phiên bản ổn định trước đó từ trang hãng GPU.'),
        (@GroupD, 5, N'Không cài được driver GPU hoặc Device Manager báo Code 43',
         N'Driver không phù hợp, phần cài đặt bị hỏng hoặc GPU/nguồn/kết nối có thể gặp lỗi.',
         N'Tải đúng driver theo model và phiên bản Windows từ trang chính hãng; gỡ/cài lại driver theo hướng dẫn của nhà sản xuất. Kiểm tra cập nhật Windows, đầu nguồn và khe GPU; nếu Code 43 vẫn còn sau cài sạch, cần kiểm tra phần cứng.'),
        (@GroupD, 6, N'Độ phân giải hoặc tần số quét không đạt mức mong muốn',
         N'Cáp/cổng kết nối, driver hoặc thiết lập hiển thị có thể chưa hỗ trợ cấu hình màn hình.',
         N'Chọn đúng độ phân giải và tần số quét trong Settings > System > Display > Advanced display; xác nhận cáp/cổng đủ băng thông và driver GPU chính hãng. Thử trực tiếp cổng khác nếu tùy chọn cần thiết không xuất hiện.'),

        /* Nhóm E — SSD, mạng và thiết bị ngoại vi */
        (@GroupE, 1, N'Windows khởi động chậm hoặc ổ đĩa thường xuyên 100%',
         N'Ổ có thể gần đầy, tác vụ nền gây tải cao hoặc SSD/HDD gặp vấn đề sức khỏe.',
         N'Kiểm tra Task Manager để tìm tiến trình gây tải; giữ dung lượng trống hợp lý và kiểm tra SMART bằng công cụ của hãng ổ đĩa. Sao lưu dữ liệu quan trọng ngay nếu có cảnh báo sức khỏe, tiếng lạ hoặc lỗi đọc/ghi.'),
        (@GroupE, 2, N'SSD không xuất hiện trong BIOS hoặc Windows',
         N'Ổ có thể chưa tiếp xúc tốt, cáp/nguồn SATA lỏng hoặc khe M.2 bị chia sẻ lane/không hỗ trợ kiểu ổ đang dùng.',
         N'Tắt máy và ngắt điện trước khi kiểm tra; gắn lại SSD M.2 hoặc cáp dữ liệu/nguồn SATA. Xem sách mainboard để xác nhận khe hỗ trợ NVMe/SATA và các hạn chế chia sẻ cổng. Không khởi tạo/format ổ nếu còn dữ liệu cần cứu.'),
        (@GroupE, 3, N'Ổ SSD gần đầy hoặc xuất hiện lỗi đọc ghi',
         N'Dung lượng trống thấp có thể làm hệ thống chậm; lỗi đọc/ghi có thể báo hiệu ổ hoặc hệ thống tệp đang gặp vấn đề.',
         N'Sao lưu trước, dọn tệp tạm và chuyển dữ liệu cá nhân sang ổ khác; kiểm tra SMART bằng tiện ích chính hãng. Nếu có cảnh báo lỗi sức khỏe hoặc tệp mất bất thường, hạn chế ghi thêm dữ liệu và cân nhắc thay ổ.'),
        (@GroupE, 4, N'Wi-Fi hoặc Ethernet kết nối mạng nhưng không vào Internet',
         N'Có thể lỗi router/ISP, cấu hình IP/DNS, cáp mạng hoặc driver adapter.',
         N'Kiểm tra thiết bị khác có vào mạng không; khởi động lại modem/router, thử cáp/cổng khác và tắt chế độ máy bay. Chạy trình khắc phục sự cố mạng; nếu chỉ máy này bị, cập nhật driver adapter và kiểm tra cấu hình proxy/VPN.'),
        (@GroupE, 5, N'Bàn phím, chuột hoặc thiết bị USB không được nhận',
         N'Cổng USB, cáp, nguồn thiết bị hoặc driver có thể gặp trục trặc.',
         N'Rút thiết bị rồi cắm lại vào cổng khác, ưu tiên cổng phía sau mainboard; tránh hub không đủ nguồn. Thử thiết bị trên máy khác để khoanh vùng, kiểm tra Device Manager và cập nhật driver chipset/USB từ hãng máy hoặc mainboard.'),
        (@GroupE, 6, N'Bluetooth, máy in hoặc thiết bị âm thanh ngoại vi không hoạt động',
         N'Thiết bị có thể chưa ghép đôi, chọn sai đầu ra mặc định hoặc driver/dịch vụ chưa hoạt động.',
         N'Kiểm tra nguồn, chế độ ghép đôi và chọn đúng thiết bị mặc định trong Settings. Xóa ghép đôi rồi kết nối lại, thử cáp/cổng khác và chạy Windows Troubleshooter; cài driver chính hãng nếu thiết bị vẫn không xuất hiện.' );

        DECLARE @InsertedIssues TABLE (NodeID INT NOT NULL, Question NVARCHAR(1000) NOT NULL);

        INSERT INTO dbo.TroubleshootingNodes (NodeType, Question, Diagnosis, [Action], IsRoot, IsActive)
        OUTPUT inserted.NodeID, inserted.Question INTO @InsertedIssues (NodeID, Question)
        SELECT 'R', Question, Diagnosis, [Action], 0, 1
        FROM @Issues;

        INSERT INTO dbo.TroubleshootingOptions (NodeID, OptionText, NextNodeID, SortOrder)
        SELECT i.GroupNodeID, i.Question, newIssue.NodeID, i.SortOrder
        FROM @Issues AS i
        JOIN @InsertedIssues AS newIssue ON newIssue.Question = i.Question;
    END;

    COMMIT TRANSACTION;
    PRINT N'Đã cập nhật cấu trúc cây chẩn đoán. Nếu cây trống, dữ liệu mẫu cho 5 nhóm đã được thêm.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
