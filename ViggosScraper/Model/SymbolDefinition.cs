namespace ViggosScraper.Model;

public class SymbolDefinition
{
    public required string Symbol { get; set; }
    public required string Description { get; set; }
    public required List<DateOnly> Dates { get; set; }
}