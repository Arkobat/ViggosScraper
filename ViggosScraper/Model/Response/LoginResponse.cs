using System.Text.Json.Serialization;

namespace ViggosScraper.Model.Response;

public class LoginResponse : StatusResponse
{
    public required string? Token { get; set; }
    public required UserDto? Profile { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required List<string>? Permissions { get; set; }
}