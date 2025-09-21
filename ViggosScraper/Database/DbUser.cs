namespace ViggosScraper.Database;

public class DbUser
{
    public int Id { get; set; }
    public required int ProfileId { get; set; }
    public required string Name { get; set; }
    public required string? RealName { get; set; }
    public required string? AvatarUrl { get; set; }
    public required string? Glass { get; set; }
    public required string? Phone { get; set; }
    
    /// <summary>
    /// How many dates the user has.
    /// </summary>
    public int TotalDatoer { get; set; } = 0;
    
    public List<DbDato> Datoer { get; set; } = null!;
    public List<Permission> Permissions { get; set; } = [];

    /// <summary>
    /// When was the user last updated by the scraper.
    /// </summary>
    public DateTimeOffset LastUpdated { get; set; }
    
    /// <summary>
    /// When was the user's data last checked (for detecting new dates without tokens).
    /// </summary>
    public DateTimeOffset? LastChecked { get; set; }
}

public class DbDato
{
    public int Id { get; set; }
    public required int Number { get; set; }
    public required DateOnly Date { get; set; }
    public required DateTime? StartDate { get; set; }
    public required DateTime? EndDate { get; set; }
    public string? Comment { get; set; }

    public int UserId { get; set; }
    public DbUser User { get; set; } = null!;
}