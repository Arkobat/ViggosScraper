namespace ViggosScraper.Database;

public class DbUser
{
    public int Id { get; set; }
    public required string ProfileId { get; set; }
    public required string Name { get; set; }
    public required string? AvatarUrl { get; set; }
    public required string? Glass { get; set; }
    
    public DateTimeOffset LastUpdated { get; set; }
}