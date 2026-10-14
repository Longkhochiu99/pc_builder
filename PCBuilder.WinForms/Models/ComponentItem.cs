namespace PCBuilder.Models;

public sealed class ComponentItem
{
    public int ComponentID { get; set; }
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = "";
    public string CategoryCode { get; set; } = "";
    public int BrandID { get; set; }
    public string BrandName { get; set; } = "";
    public string ModelName { get; set; } = "";
    public string? SKU { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public string? Socket { get; set; }
    public string? MemoryType { get; set; }
    public int? TdpW { get; set; }
    public int? PowerDrawW { get; set; }
    public int? WattageW { get; set; }
    public decimal? LengthMM { get; set; }
    public decimal? MaxGpuLengthMM { get; set; }
    public decimal? MaxCoolerHeightMM { get; set; }
    public string? FormFactor { get; set; }
    public decimal? CpuBenchmarkScore { get; set; }
    public decimal? GpuBenchmarkScore { get; set; }
    public string DisplayName => $"{BrandName} {ModelName}";
    public override string ToString() => DisplayName;
}
