using PCBuilder.Models;

namespace PCBuilder.Services;

public sealed record CompatibilityResult(string Severity, string Message);

public sealed class CompatibilityService
{
    public List<CompatibilityResult> Check(IReadOnlyCollection<ComponentItem> items)
    {
        var result = new List<CompatibilityResult>();
        var cpu = items.FirstOrDefault(x => x.CategoryCode == "CPU");
        var board = items.FirstOrDefault(x => x.CategoryCode == "MOTHERBOARD");
        var ram = items.FirstOrDefault(x => x.CategoryCode == "RAM");
        var gpu = items.FirstOrDefault(x => x.CategoryCode == "GPU");
        var psu = items.FirstOrDefault(x => x.CategoryCode == "PSU");
        var pcCase = items.FirstOrDefault(x => x.CategoryCode == "CASE");
        if (cpu is not null && board is not null)
        {
            var cpuSocket = RecommendationService.ResolveSocket(cpu);
            var boardSocket = RecommendationService.ResolveSocket(board);
            if (cpuSocket is null || boardSocket is null) result.Add(new("Warning", "Thiếu dữ liệu socket CPU/Mainboard; chưa thể kết luận tương thích."));
            else if (!cpuSocket.Equals(boardSocket, StringComparison.OrdinalIgnoreCase)) result.Add(new("Error", $"Socket không khớp: CPU {cpuSocket}, Mainboard {boardSocket}."));
            else result.Add(new("Info", $"Socket CPU/Mainboard khớp ({cpuSocket})."));
        }
        if (ram is not null && board is not null)
        {
            if (ram.MemoryType is null || board.MemoryType is null) result.Add(new("Warning", "Thiếu dữ liệu chuẩn RAM; chưa thể kết luận."));
            else if (!ram.MemoryType.Equals(board.MemoryType, StringComparison.OrdinalIgnoreCase)) result.Add(new("Error", $"Chuẩn RAM không khớp: RAM {ram.MemoryType}, Mainboard {board.MemoryType}."));
            else result.Add(new("Info", $"Chuẩn RAM khớp ({ram.MemoryType})."));
        }
        if (gpu is not null && pcCase is not null)
        {
            if (!gpu.LengthMM.HasValue || !pcCase.MaxGpuLengthMM.HasValue) result.Add(new("Warning", "Thiếu dữ liệu chiều dài GPU hoặc giới hạn Case."));
            else if (gpu.LengthMM.Value > pcCase.MaxGpuLengthMM.Value) result.Add(new("Error", $"GPU dài {gpu.LengthMM.Value} mm, vượt giới hạn Case {pcCase.MaxGpuLengthMM.Value} mm."));
            else result.Add(new("Info", $"GPU vừa giới hạn chiều dài Case ({gpu.LengthMM.Value}/{pcCase.MaxGpuLengthMM.Value} mm)."));
        }
        int estimatedW = (cpu?.TdpW ?? 0) + (gpu?.PowerDrawW ?? 0) + items.Where(x => x.CategoryCode is "STORAGE" or "RAM" or "COOLING").Count() * 15 + 60;
        if (psu is not null && psu.WattageW.HasValue)
        {
            int recommended = (int)Math.Ceiling(estimatedW * 1.3);
            result.Add(psu.WattageW.Value < recommended
                ? new("Warning", $"Nguồn {psu.WattageW} W có thể không đủ. Công suất ước tính {estimatedW} W; khuyến nghị tối thiểu khoảng {recommended} W (đã cộng 30% dự phòng).")
                : new("Info", $"Nguồn {psu.WattageW} W đạt mức dự phòng sơ bộ. Công suất ước tính {estimatedW} W."));
        }
        else result.Add(new("Warning", "Chưa chọn PSU hoặc thiếu dữ liệu công suất PSU."));
        if (result.Count == 0) result.Add(new("Info", "Chọn thêm linh kiện để kiểm tra tương thích."));
        return result;
    }

    public string EstimateBottleneck(ComponentItem? cpu, ComponentItem? gpu)
    {
        if (cpu is null || gpu is null) return "Chọn cả CPU và GPU để xem cảnh báo sơ bộ.";
        // Heuristic placeholder: no benchmark database is present; do not treat as measured performance.
        int cpuScore = Math.Max(1, (cpu.TdpW ?? 65));
        int gpuScore = Math.Max(1, (gpu.PowerDrawW ?? 150));
        double ratio = (double)cpuScore / gpuScore;
        double deviation = Math.Min(35, Math.Abs(1.0 - ratio) * 35);
        string side = ratio < 0.65 ? "CPU có thể hạn chế GPU" : ratio > 1.45 ? "GPU có thể là giới hạn chính" : "Tương quan công suất khá cân bằng";
        return $"Ước lượng rất sơ bộ: {deviation:0}% chênh lệch theo proxy công suất — {side}. Đây KHÔNG phải bottleneck đo bằng benchmark; TDP/điện năng không đại diện trực tiếp cho hiệu năng.";
    }
}
