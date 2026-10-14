using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace PCBuilder.Data;

public static class Db
{
    private static string? _connectionString;
    public static string ConnectionString
    {
        get
        {
            if (_connectionString is not null) return _connectionString;
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(path)) throw new FileNotFoundException("Không tìm thấy appsettings.json.", path);
            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            _connectionString = doc.RootElement.GetProperty("ConnectionStrings").GetProperty("PCBuilderDB").GetString();
            if (string.IsNullOrWhiteSpace(_connectionString)) throw new InvalidOperationException("Connection string đang trống.");
            return _connectionString;
        }
    }
    public static SqlConnection CreateConnection() => new(ConnectionString);
}
