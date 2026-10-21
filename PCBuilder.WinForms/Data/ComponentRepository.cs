using Microsoft.Data.SqlClient;
using PCBuilder.Models;
using System.Data;

namespace PCBuilder.Data;

public sealed class ComponentRepository
{
    public List<ComponentItem> Search(string? keyword = null, int? categoryId = null, bool includeInactive = false)
    {
        using var conn = Db.CreateConnection();
        using var cmd = new SqlCommand("dbo.sp_Component_Search", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 200).Value = string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : keyword.Trim();
        cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Value = categoryId.HasValue ? categoryId.Value : DBNull.Value;
        cmd.Parameters.Add("@IncludeInactive", SqlDbType.Bit).Value = includeInactive;
        conn.Open(); using var r = cmd.ExecuteReader(); var list = new List<ComponentItem>();
        while (r.Read()) list.Add(Map(r));
        return list;
    }

    public void Save(ComponentItem c)
    {
        using var conn = Db.CreateConnection();
        using var cmd = new SqlCommand(c.ComponentID == 0 ? "dbo.sp_Component_Create" : "dbo.sp_Component_Update", conn) { CommandType = CommandType.StoredProcedure };
        if (c.ComponentID != 0) cmd.Parameters.Add("@ComponentID", SqlDbType.Int).Value = c.ComponentID;
        cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Value = c.CategoryID;
        cmd.Parameters.Add("@BrandID", SqlDbType.Int).Value = c.BrandID;
        cmd.Parameters.Add("@ModelName", SqlDbType.NVarChar, 200).Value = c.ModelName.Trim();
        cmd.Parameters.Add("@SKU", SqlDbType.NVarChar, 100).Value = (object?)c.SKU ?? DBNull.Value;
        cmd.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value = DBNull.Value;
        cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = c.Price;
        cmd.Parameters["@Price"].Precision = 18; cmd.Parameters["@Price"].Scale = 2;
        cmd.Parameters.Add("@StockQuantity", SqlDbType.Int).Value = c.StockQuantity;
        cmd.Parameters.Add("@ImageURL", SqlDbType.NVarChar, 1000).Value = DBNull.Value;
        if (c.ComponentID != 0) cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = c.IsActive;
        conn.Open(); cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = Db.CreateConnection(); using var cmd = new SqlCommand("dbo.sp_Component_Delete", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.Add("@ComponentID", SqlDbType.Int).Value = id; conn.Open(); cmd.ExecuteNonQuery();
    }

    public DataTable GetCategories()
    {
        using var conn = Db.CreateConnection(); using var cmd = new SqlCommand("SELECT CategoryID, CategoryName FROM dbo.Categories WHERE IsActive=1 ORDER BY CategoryName", conn);
        using var da = new SqlDataAdapter(cmd); var dt = new DataTable(); da.Fill(dt); return dt;
    }
    public DataTable GetBrands()
    {
        using var conn = Db.CreateConnection(); using var cmd = new SqlCommand("SELECT BrandID, BrandName FROM dbo.Brands WHERE IsActive=1 ORDER BY BrandName", conn);
        using var da = new SqlDataAdapter(cmd); var dt = new DataTable(); da.Fill(dt); return dt;
    }

    private static ComponentItem Map(SqlDataReader r)
    {
        var item = new ComponentItem
        {
            ComponentID = Convert.ToInt32(r["ComponentID"]), CategoryID = Convert.ToInt32(r["CategoryID"]), CategoryName = Convert.ToString(r["CategoryName"]) ?? "",
            CategoryCode = Convert.ToString(r["CategoryCode"]) ?? "", BrandID = Convert.ToInt32(r["BrandID"]), BrandName = Convert.ToString(r["BrandName"]) ?? "",
            ModelName = Convert.ToString(r["ModelName"]) ?? "", SKU = r["SKU"] == DBNull.Value ? null : Convert.ToString(r["SKU"]), Price = Convert.ToDecimal(r["Price"]),
            StockQuantity = Convert.ToInt32(r["StockQuantity"]), IsActive = Convert.ToBoolean(r["IsActive"]),
            Socket = ValString(r,"Socket"), MemoryType = ValString(r,"MemoryType"), TdpW = ValInt(r,"TDP_W"), PowerDrawW = ValInt(r,"PowerDraw_W"), WattageW = ValInt(r,"Wattage_W"),
            LengthMM = ValDecimal(r,"LengthMM"), MaxGpuLengthMM = ValDecimal(r,"MaxGPULengthMM"), MaxCoolerHeightMM = ValDecimal(r,"MaxCoolerHeightMM"), FormFactor = ValString(r,"FormFactor"), CpuBenchmarkScore = ValDecimal(r,"CpuBenchmarkScore"), GpuBenchmarkScore = ValDecimal(r,"GpuBenchmarkScore")
        };
        if (string.IsNullOrWhiteSpace(item.Socket))
        {
            item.Socket = PCBuilder.Services.RecommendationService.ResolveSocket(item);
        }
        return item;
    }
    private static string? ValString(SqlDataReader r,string n) => r[n] == DBNull.Value ? null : Convert.ToString(r[n]);
    private static int? ValInt(SqlDataReader r,string n) => r[n] == DBNull.Value ? null : Convert.ToInt32(r[n]);
    private static decimal? ValDecimal(SqlDataReader r,string n) => r[n] == DBNull.Value ? null : Convert.ToDecimal(r[n]);
}
