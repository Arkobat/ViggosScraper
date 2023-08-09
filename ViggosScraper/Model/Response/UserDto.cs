namespace ViggosScraper.Model;

public class UserDto
{
    public required string ProfileId { get; set; }
    public required string Name { get; set; }
    public required string? AvatarUrl { get; set; }
    public required string? Krus { get; set; }
    public int TotalDates => Dates.Count;
    public required List<Dato> Dates { get; set; } = new();
}