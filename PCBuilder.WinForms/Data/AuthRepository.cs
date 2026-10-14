using Microsoft.Data.SqlClient;
using PCBuilder.Models;
using System.Security.Cryptography;

namespace PCBuilder.Data;

public sealed class AuthRepository
{
    public AppUser? Login(string username, string password)
    {
        using var conn = Db.CreateConnection();
        using var cmd = new SqlCommand("dbo.sp_User_GetForLogin", conn) { CommandType = System.Data.CommandType.StoredProcedure };
        cmd.Parameters.Add("@Username", System.Data.SqlDbType.NVarChar, 50).Value = username.Trim();
        conn.Open();
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        var hash = r.GetString(r.GetOrdinal("PasswordHash"));
        if (!PasswordHash.Verify(password, hash)) return null;
        return new AppUser(r.GetInt32(r.GetOrdinal("UserID")), r.GetString(r.GetOrdinal("Username")), r.IsDBNull(r.GetOrdinal("FullName")) ? username : r.GetString(r.GetOrdinal("FullName")), r.GetString(r.GetOrdinal("UserRole")));
    }

    public void RegisterUser(string username, string email, string fullName, string password)
    {
        using var conn = Db.CreateConnection();
        using var cmd = new SqlCommand("dbo.sp_User_Create", conn) { CommandType = System.Data.CommandType.StoredProcedure };
        cmd.Parameters.Add("@Username", System.Data.SqlDbType.NVarChar, 50).Value = username.Trim();
        cmd.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar, 255).Value = email.Trim();
        cmd.Parameters.Add("@PasswordHash", System.Data.SqlDbType.NVarChar, 255).Value = PasswordHash.Create(password);
        cmd.Parameters.Add("@FullName", System.Data.SqlDbType.NVarChar, 150).Value = fullName.Trim();
        cmd.Parameters.Add("@UserRole", System.Data.SqlDbType.VarChar, 10).Value = "User";
        conn.Open(); cmd.ExecuteNonQuery();
    }


}

public static class PasswordHash
{
    // PBKDF2-SHA256; encoded format: PBKDF2$iterations$salt$hash
    private const int Iterations = 210_000;
    public static string Create(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return $"PBKDF2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }
    public static bool Verify(string password, string encoded)
    {
        try
        {
            var parts = encoded.Split('$');
            if (parts.Length != 4 || parts[0] != "PBKDF2") return false;
            int iterations = int.Parse(parts[1]);
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch { return false; }
    }
}
