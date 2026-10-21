using PCBuilder.Models;

namespace PCBuilder.Services;

public enum BuildPurpose { Gaming, Productivity, Balanced }
public sealed record RecommendedBuild(IReadOnlyList<ComponentItem> Components, decimal TotalPrice, decimal PerformanceScore, decimal ValueScore, string Notes);

/// <summary>
/// Benchmark-aware budget recommendation. It uses beam search to avoid brute-force Cartesian explosion.
/// Scores are relative indices from BenchmarkScores; demo values are explicitly unverified.
/// </summary>
public sealed class RecommendationService
{
    private static readonly string[] Required = ["CPU", "MOTHERBOARD", "RAM", "GPU", "PSU", "STORAGE", "CASE"];

    public IReadOnlyList<RecommendedBuild> Recommend(IEnumerable<ComponentItem> source, decimal budget, BuildPurpose purpose, int resultCount = 3)
    {
        if (budget <= 0) throw new ArgumentOutOfRangeException(nameof(budget), "Ngân sách phải lớn hơn 0.");
        var all = source.Where(x => x.IsActive && x.Price >= 0 && x.StockQuantity > 0).ToList();
        var byCategory = Required.ToDictionary(code => code, code => all.Where(x => x.CategoryCode.Equals(code, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => ComponentScore(x, code, purpose)).ThenBy(x => x.Price).Take(18).ToList());
        var missing = Required.Where(c => byCategory[c].Count == 0).ToArray();
        if (missing.Length > 0) throw new InvalidOperationException("Thiếu linh kiện còn hàng ở các nhóm: " + string.Join(", ", missing) + ".");

        var beam = new List<List<ComponentItem>> { new() };
        foreach (var category in Required)
        {
            var next = new List<List<ComponentItem>>();
            foreach (var partial in beam)
            foreach (var item in byCategory[category])
            {
                var total = partial.Sum(x => x.Price) + item.Price;
                if (total > budget) continue;
                var candidate = partial.Append(item).ToList();
                if (!PartialCompatible(candidate)) continue;
                next.Add(candidate);
            }
            beam = next.OrderByDescending(x => RankPartial(x, purpose, budget)).Take(250).ToList();
            if (beam.Count == 0) break;
        }

        var results = beam.Where(x => x.Count == Required.Length && FullyCompatible(x))
            .Select(x => {
                var price = x.Sum(c => c.Price);
                var perf = Performance(x, purpose);
                var value = perf / (double)Math.Max(1m, price / 1_000_000m);
                var unverified = x.Where(c => (c.CategoryCode == "CPU" && c.CpuBenchmarkScore is null) || (c.CategoryCode == "GPU" && c.GpuBenchmarkScore is null)).Any();
                var notes = unverified ? "Thiếu benchmark CPU/GPU; điểm hiệu năng chỉ là ước lượng thiếu dữ liệu." : "Xếp hạng theo benchmark đã nhập và mục đích sử dụng; hãy xác minh nguồn điểm benchmark.";
                return new RecommendedBuild(x, price, (decimal)perf, (decimal)value, notes);
            })
            .OrderByDescending(x => x.PerformanceScore).ThenByDescending(x => x.ValueScore).Take(Math.Clamp(resultCount, 1, 10)).ToList();
        if (results.Count == 0) throw new InvalidOperationException("Không tìm thấy cấu hình đầy đủ, tương thích và nằm trong ngân sách. Hãy tăng ngân sách hoặc bổ sung dữ liệu kỹ thuật/linh kiện.");
        return results;
    }

    public static string? ResolveSocket(ComponentItem? item)
    {
        if (item is null) return null;
        if (!string.IsNullOrWhiteSpace(item.Socket)) return item.Socket.Trim();

        var text = $"{item.ModelName} {item.SKU}".ToUpperInvariant();
        if (text.Contains("AM5") || text.Contains("B650") || text.Contains("X670") || text.Contains("B850") || text.Contains("X870") || text.Contains("A620")) return "AM5";
        if (text.Contains("AM4") || text.Contains("B550") || text.Contains("X570") || text.Contains("B450") || text.Contains("A520") || text.Contains("A320")) return "AM4";
        if (text.Contains("LGA1700") || text.Contains("1700") || text.Contains("B760") || text.Contains("Z790") || text.Contains("B660") || text.Contains("Z690") || text.Contains("H610")) return "LGA1700";
        if (text.Contains("LGA1200") || text.Contains("1200") || text.Contains("B560") || text.Contains("Z590") || text.Contains("B460") || text.Contains("H410") || text.Contains("H510")) return "LGA1200";
        if (text.Contains("LGA1151") || text.Contains("1151") || text.Contains("B360") || text.Contains("Z390") || text.Contains("B365") || text.Contains("H310")) return "LGA1151";

        return null;
    }

    private static double ComponentScore(ComponentItem c, string category, BuildPurpose p) => category switch
    {
        "CPU" => (double)(c.CpuBenchmarkScore ?? (c.Price > 0 ? c.Price / 1000m : 0m)),
        "GPU" => (double)(c.GpuBenchmarkScore ?? (c.Price > 0 ? c.Price / 1000m : 0m)),
        _ => c.Price > 0 ? 1.0 / (double)c.Price : 0
    };

    private static double Performance(IReadOnlyCollection<ComponentItem> items, BuildPurpose purpose)
    {
        var cpuItem = items.First(x => x.CategoryCode == "CPU");
        var gpuItem = items.First(x => x.CategoryCode == "GPU");
        var ram = items.First(x => x.CategoryCode == "RAM");
        var storage = items.First(x => x.CategoryCode == "STORAGE");

        var cpu = (double)(cpuItem.CpuBenchmarkScore ?? (cpuItem.Price > 0 ? cpuItem.Price / 1000m : 5000m));
        var gpu = (double)(gpuItem.GpuBenchmarkScore ?? (gpuItem.Price > 0 ? gpuItem.Price / 1000m : 8000m));

        var memoryBonus = Math.Min(1.15, 1.0 + Math.Max(0.0, (double)ram.Price) / 100_000_000.0);
        var storageBonus = storage.Price > 0 ? 1.03 : 1.0;
        var weighted = purpose switch
        {
            BuildPurpose.Gaming => gpu * 0.72 + cpu * 0.28,
            BuildPurpose.Productivity => cpu * 0.72 + gpu * 0.28,
            _ => cpu * 0.5 + gpu * 0.5
        };
        return weighted * memoryBonus * storageBonus;
    }

    private static double RankPartial(IReadOnlyCollection<ComponentItem> items, BuildPurpose purpose, decimal budget)
    {
        var perf = PerformancePartial(items, purpose);
        var cost = Math.Max(1.0, (double)items.Sum(x => x.Price));
        // Performance dominates; gently penalize cost to retain room for remaining required parts.
        return perf / Math.Pow(cost, 0.15) - (double)items.Sum(x => x.Price / budget) * 0.001;
    }
    private static double PerformancePartial(IReadOnlyCollection<ComponentItem> items, BuildPurpose p)
    {
        var cpuItem = items.FirstOrDefault(x => x.CategoryCode == "CPU");
        var gpuItem = items.FirstOrDefault(x => x.CategoryCode == "GPU");
        var cpu = (double)(cpuItem?.CpuBenchmarkScore ?? (cpuItem?.Price > 0 ? cpuItem.Price / 1000m : 0m));
        var gpu = (double)(gpuItem?.GpuBenchmarkScore ?? (gpuItem?.Price > 0 ? gpuItem.Price / 1000m : 0m));
        return p switch { BuildPurpose.Gaming => gpu * .72 + cpu * .28, BuildPurpose.Productivity => cpu * .72 + gpu * .28, _ => cpu * .5 + gpu * .5 };
    }

    private static bool PartialCompatible(IReadOnlyCollection<ComponentItem> x)
    {
        var cpu = x.FirstOrDefault(c => c.CategoryCode == "CPU");
        var mb = x.FirstOrDefault(c => c.CategoryCode == "MOTHERBOARD");
        var ram = x.FirstOrDefault(c => c.CategoryCode == "RAM");
        var gpu = x.FirstOrDefault(c => c.CategoryCode == "GPU");
        var pcCase = x.FirstOrDefault(c => c.CategoryCode == "CASE");

        if (cpu is not null && mb is not null)
        {
            var cpuSocket = ResolveSocket(cpu);
            var mbSocket = ResolveSocket(mb);
            if (!string.IsNullOrEmpty(cpuSocket) && !string.IsNullOrEmpty(mbSocket) &&
                !cpuSocket.Equals(mbSocket, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        if (ram is not null && mb is not null &&
            !string.IsNullOrWhiteSpace(ram.MemoryType) && !string.IsNullOrWhiteSpace(mb.MemoryType) &&
            !ram.MemoryType.Equals(mb.MemoryType, StringComparison.OrdinalIgnoreCase))
            return false;

        if (cpu is not null && !string.IsNullOrWhiteSpace(cpu.MemoryType))
        {
            if (ram is not null && !string.IsNullOrWhiteSpace(ram.MemoryType) &&
                !cpu.MemoryType.Equals(ram.MemoryType, StringComparison.OrdinalIgnoreCase))
                return false;

            if (mb is not null && !string.IsNullOrWhiteSpace(mb.MemoryType) &&
                !cpu.MemoryType.Equals(mb.MemoryType, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        if (gpu is not null && pcCase is not null && gpu.LengthMM.HasValue && pcCase.MaxGpuLengthMM.HasValue &&
            gpu.LengthMM.Value > pcCase.MaxGpuLengthMM.Value)
            return false;

        return true;
    }

    private static bool FullyCompatible(IReadOnlyCollection<ComponentItem> x)
    {
        if (!PartialCompatible(x)) return false;

        var cpu = x.FirstOrDefault(c => c.CategoryCode == "CPU");
        var mb = x.FirstOrDefault(c => c.CategoryCode == "MOTHERBOARD");
        var ram = x.FirstOrDefault(c => c.CategoryCode == "RAM");
        var gpu = x.FirstOrDefault(c => c.CategoryCode == "GPU");
        var psu = x.FirstOrDefault(c => c.CategoryCode == "PSU");
        var pcCase = x.FirstOrDefault(c => c.CategoryCode == "CASE");

        if (cpu is null || mb is null || ram is null || gpu is null || psu is null || pcCase is null)
            return false;

        var cpuSocket = ResolveSocket(cpu);
        var mbSocket = ResolveSocket(mb);
        if (!string.IsNullOrEmpty(cpuSocket) && !string.IsNullOrEmpty(mbSocket) &&
            !cpuSocket.Equals(mbSocket, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(ram.MemoryType) && !string.IsNullOrWhiteSpace(mb.MemoryType) &&
            !ram.MemoryType.Equals(mb.MemoryType, StringComparison.OrdinalIgnoreCase))
            return false;

        if (gpu.LengthMM.HasValue && pcCase.MaxGpuLengthMM.HasValue &&
            gpu.LengthMM.Value > pcCase.MaxGpuLengthMM.Value)
            return false;

        if (psu.WattageW.HasValue)
        {
            int cpuTdp = cpu.TdpW ?? 65;
            int gpuPower = gpu.PowerDrawW ?? 150;
            int estimatedW = cpuTdp + gpuPower + 100;
            int recommendedMinPsu = (int)Math.Ceiling(estimatedW * 1.3m);
            if (psu.WattageW.Value < recommendedMinPsu)
                return false;
        }

        return true;
    }
}
