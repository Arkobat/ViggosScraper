using System.Text.Json.Serialization;

namespace ViggosScraper.Model.Response;

public class Dato
{
    public required int Number { get; set; }
    public required DateOnly Date { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required SimpleDatoSymbol? Symbol { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required DateTimeOffset? Start { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required DateTimeOffset? Finish { get; set; }
}