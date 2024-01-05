using System.Text.Json.Serialization;

namespace ViggosScraper.Model.Response;

public class Dato
{
    public int Number { get; set; }
    public DateOnly Date { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SimpleDatoSymbol? Symbol { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? Start { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? Finish { get; set; }
}