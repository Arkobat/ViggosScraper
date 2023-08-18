namespace ViggosScraper.Database;

public class DbUser
{
    public int Id { get; set; }
    public required string ProfileId { get; set; }
    public required string Name { get; set; }
    public required string? AvatarUrl { get; set; }
    public required string? Glass { get; set; }
    public List<DbDato> Datoer { get; set; } = null!;

    public DateTimeOffset LastUpdated { get; set; }
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