namespace PCBuilder.Models;

public sealed record AppUser(int UserId, string Username, string FullName, string Role)
{
    public bool IsAdmin => string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase);
}
