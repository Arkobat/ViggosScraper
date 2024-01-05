namespace ViggosScraper.Model.Response;

public class BattleDto
{
    public required int BattleId { get; set; }
    public required bool Confirmed { get; set; }
    public required DateTimeOffset Time { get; set; }
    public required List<string> Winners { get; set; } = null!;
    public required List<string> Losers { get; set; } = null!;
}