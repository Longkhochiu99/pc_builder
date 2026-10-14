using Microsoft.Data.SqlClient;
using PCBuilder.Models;
using System.Data;

namespace PCBuilder.Data;

public sealed class BuildRepository
{
    public int Save(int userId, string name, decimal? budget, IReadOnlyCollection<ComponentItem> items)
    {
        if (items.Count == 0) throw new InvalidOperationException("Cấu hình chưa có linh kiện.");
        using var conn = Db.CreateConnection(); conn.Open(); using var tx = conn.BeginTransaction();
        try
        {
            int buildId;
            using (var cmd = new SqlCommand("INSERT dbo.PCBuilds(UserID,BuildName,Budget,Status) OUTPUT INSERTED.BuildID VALUES(@UserID,@BuildName,@Budget,'Saved')", conn, tx))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId; cmd.Parameters.Add("@BuildName", SqlDbType.NVarChar, 150).Value = name;
                cmd.Parameters.Add("@Budget", SqlDbType.Decimal).Value = budget.HasValue ? budget.Value : DBNull.Value; cmd.Parameters["@Budget"].Precision = 18; cmd.Parameters["@Budget"].Scale = 2;
                buildId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            foreach (var item in items)
            {
                using var cmd = new SqlCommand("INSERT dbo.PCBuildItems(BuildID,ComponentID,Quantity,UnitPriceSnapshot) VALUES(@BuildID,@ComponentID,1,@Price)", conn, tx);
                cmd.Parameters.Add("@BuildID", SqlDbType.Int).Value = buildId; cmd.Parameters.Add("@ComponentID", SqlDbType.Int).Value = item.ComponentID;
                cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = item.Price; cmd.Parameters["@Price"].Precision = 18; cmd.Parameters["@Price"].Scale = 2; cmd.ExecuteNonQuery();
            }
            tx.Commit(); return buildId;
        }
        catch { tx.Rollback(); throw; }
    }
}
