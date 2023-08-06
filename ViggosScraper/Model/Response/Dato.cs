using System.Text.Json.Serialization;

namespace ViggosScraper.Model;

public class Dato
{
    public int Number { get; set; }
    public DateOnly Date { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SimpleDatoSymbol? Symbol { get; set; }
}

public class DatoSymbol
{
    public string Symbol { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public DateOnly Date { get; set; }
    
    public SimpleDatoSymbol Simple() => new()
    {
        Symbol = Symbol,
        Reason = Reason
    };
}


public class SimpleDatoSymbol
{
    public string Symbol { get; set; } = null!;
    public string Reason { get; set; } = null!;
}