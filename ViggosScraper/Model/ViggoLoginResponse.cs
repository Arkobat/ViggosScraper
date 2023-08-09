using System.Text.Json.Serialization;

namespace ViggosScraper.Model;

public class ViggoLoginResponse
{
    public int Status { get; set; }
    public string Msg { get; set; } = null!;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Player? Player { get; set; }
}

public class Player
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Alias { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string? Photo { get; set; }
    public string? GlassNumber { get; set; } = null!;
    public List<ViggoDato> Dates { get; set; } = null!;
}

public class ViggoDato
{
    [JsonPropertyName("dateid")]public string DateId { get; set; } = null!;
    [JsonPropertyName("comment")]public string Comment { get; set; } = null!;
    [JsonPropertyName("status")]public string Status { get; set; } = null!;
    [JsonPropertyName("enddate")]public string EndDate { get; set; } = null!;
    [JsonPropertyName("date_formatted")]public string DateFormatted { get; set; } = null!;
    [JsonPropertyName("enddate_formatted")]public string EndDateFormatted { get; set; } = null!;
}

