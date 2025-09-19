using System.Text.Json.Serialization;

namespace DrikDatoApp.Model;

public class HighcoreDto
{
    [JsonPropertyName("status")]
    public required int Status { get; set; }
    
    [JsonPropertyName("msg")]
    public required string Message { get; set; }
    
    [JsonPropertyName("scores")]
    public required List<HighscoreEntry> Highscores { get; set; }
}

public class HighscoreEntry
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }
    
    [JsonPropertyName("alias")]
    public required string Name { get; set; }
    
    [JsonPropertyName("antalDatoer")]
    public required int TotalDates { get; set; }
}