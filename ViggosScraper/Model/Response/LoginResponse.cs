using System.Text.Json.Serialization;

namespace ViggosScraper.Model;

public class LoginResponse
{
    public required bool Success { get; set; }
    public required string Message { get; set; } = null!;
    public string? Token { get; set; }
    public User? Profile { get; set; }
    public List<SearchResult> Friends { get; set; } = new();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<SearchResult>? FriendRequests { get; set; }
}