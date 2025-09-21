using System.Text.Json.Serialization;

namespace DrikDatoApp.Model;

public class SearchDto
{
    [JsonPropertyName("status")]
    public required int Status { get; set; }
    
    [JsonPropertyName("msg")]
    public required string Message { get; set; }
    
    [JsonPropertyName("results")]
    public required List<SearchResult> Results { get; set; }
}

public class SearchResult
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("alias")]
    public required string Alias { get; set; }
}
