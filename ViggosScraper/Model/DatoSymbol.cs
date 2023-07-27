namespace ViggosScraper.Model;

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