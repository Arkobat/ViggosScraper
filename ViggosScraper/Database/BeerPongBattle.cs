namespace ViggosScraper.Database;

public class BeerPongBattle
{
    public int BattleId { get; set; }
    public DateTimeOffset Time { get; set; }
    public bool Confirmed { get; set; }
    public List<BattleResult> Results { get; set; } = null!;
}

public class BattleResult
{
    public int ResultId { get; set; }
    public bool Won { get; set; }
    
    public int BattleId { get; set; }
    public BeerPongBattle Battle { get; set; } = null!;
    
    public int UserId { get; set; }
    public DbUser User { get; set; } = null!;
}
