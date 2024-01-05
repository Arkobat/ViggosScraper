namespace ViggosScraper.Database;

public class DbUser
{
    public int Id { get; set; }
    public required string ProfileId { get; set; }
    public required string Name { get; set; }
    public required string? AvatarUrl { get; set; }
    public required string? Glass { get; set; }
    public List<DbDato> Datoer { get; set; } = null!;
    public List<BattleResult> Battles { get; set; } = null!;
    public List<Permission> Permissions { get; set; } = new();

    public DateTimeOffset LastUpdated { get; set; }

    public bool HasPermission(string permission) => Permissions.Any(p => p.Name == permission);
}

public class DbDato
{
    public int Id { get; set; }
    public int Number { get; set; }
    public DateOnly Date { get; set; }
    public string? Comment { get; set; }

    public int UserId { get; set; }
    public DbUser User { get; set; } = null!;
}