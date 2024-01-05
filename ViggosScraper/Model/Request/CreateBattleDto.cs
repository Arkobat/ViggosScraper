namespace ViggosScraper.Model.Request;

public class CreateBattleDto
{
    public required List<string> Winners { get; set; } = null!;
    public required List<string> Losers { get; set; } = null!;
}