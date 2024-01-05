namespace ViggosScraper.Database;

public class Permission
{
    public int PermissionId { get; set; }
    public string Name { get; set; } = null!;

    public int UserId { get; set; }
    public DbUser User { get; set; } = null!;
}