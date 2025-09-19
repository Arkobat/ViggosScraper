namespace ViggosScraper.Model.Response;

public class HighscoreEntry
{
    public required int Position { get; set; }
    public required string Name { get; set; }
    public required string ProfileId { get; set; }
    public required int TotalDates { get; set; }
}
