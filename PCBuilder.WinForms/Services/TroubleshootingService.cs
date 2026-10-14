namespace PCBuilder.Services;

public sealed record TroubleStep(string Question, string[] Options, int[] Next, string? Diagnosis = null, string? Action = null);

public sealed class TroubleshootingService
{
    private readonly Dictionary<int, TroubleStep> _steps = new()
    {
        [0] = new("Máy tính gặp triệu chứng nào?", new[] { "Không lên nguồn", "Quạt quay nhưng không lên hình", "Màn hình xanh / tự khởi động lại" }, new[] { 1, 2, 3 }),
        [1] = new("Khi bấm nút nguồn, quạt hoặc đèn có hoạt động không?", new[] { "Không có dấu hiệu nào", "Quạt/đèn có hoạt động" }, new[] { 4, 5 }),
        [2] = new("Bạn đã kiểm tra màn hình và dây tín hiệu chưa?", new[] { "Chưa / chưa chắc", "Đã kiểm tra, vẫn không có hình" }, new[] { 6, 7 }),
        [3] = new("Lỗi có xuất hiện sau khi cài driver hoặc cập nhật hệ thống không?", new[] { "Có", "Không / không rõ" }, new[] { 8, 9 }),
        [4] = new("Kết luận: khả năng liên quan nguồn điện hoặc PSU.", Array.Empty<string>(), Array.Empty<int>(), "Máy không có dấu hiệu cấp điện.", "Kiểm tra ổ cắm, dây nguồn, công tắc PSU và đầu cắm 24-pin/CPU. Ngắt điện trước khi thao tác; nếu vẫn không hoạt động, nhờ kỹ thuật viên kiểm tra PSU."),
        [5] = new("Kết luận: có điện nhưng hệ thống không khởi động hoàn tất.", Array.Empty<string>(), Array.Empty<int>(), "Có thể do RAM, mainboard, CPU hoặc dây nguồn phụ.", "Tắt nguồn, rút điện; thử lắp lại RAM từng thanh và kiểm tra đèn debug/beep code. Không tháo linh kiện khi máy còn điện."),
        [6] = new("Kết luận: ưu tiên kiểm tra đường hiển thị.", Array.Empty<string>(), Array.Empty<int>(), "Có thể do input màn hình, cáp hoặc cắm nhầm cổng xuất hình.", "Chọn đúng input màn hình, thử cáp/cổng khác; nếu CPU không có iGPU, cắm màn hình vào cổng trên GPU rời."),
        [7] = new("Kết luận: hệ thống POST hoặc GPU có thể gặp lỗi.", Array.Empty<string>(), Array.Empty<int>(), "Có thể do GPU, RAM, nguồn phụ GPU hoặc BIOS.", "Kiểm tra đầu nguồn GPU, reseat GPU/RAM khi đã ngắt điện; xem debug LED và thử cấu hình tối thiểu."),
        [8] = new("Kết luận: có khả năng liên quan phần mềm/driver.", Array.Empty<string>(), Array.Empty<int>(), "Driver hoặc bản cập nhật gần đây có thể gây lỗi.", "Khởi động Safe Mode nếu có thể, rollback driver/update gần nhất và kiểm tra mã lỗi trong Event Viewer."),
        [9] = new("Kết luận: cần kiểm tra mã lỗi và phần cứng.", Array.Empty<string>(), Array.Empty<int>(), "Nguyên nhân chưa xác định; có thể là RAM, nhiệt độ, ổ đĩa hoặc driver.", "Ghi lại stop code, kiểm tra nhiệt độ, SMART ổ đĩa và Windows Memory Diagnostic. Sao lưu dữ liệu quan trọng.")
    };
    public TroubleStep Get(int id) => _steps[id];
    public int Follow(int id, int optionIndex) => _steps[id].Next[optionIndex];
    public bool IsTerminal(int id) => _steps[id].Diagnosis is not null;
    public int Root => 0;
}
