using System.ComponentModel.DataAnnotations;
using ViggosScraper.Model;

namespace ViggosScraper.Database;

public class DbUser
{
    [Key] public int Id { get; set; }
    public required string ProfileId { get; set; }
    public required string Name { get; set; }
    public required string? AvatarUrl { get; set; }
    public required string Krus { get; set; }
    public required List<DbDato> Dates { get; set; } = new();
    public DateTimeOffset LastUpdated { get; set; }
    public List<DbFriendConnection> Friends { get; set; } = new();
}

public class DbDato
{
    [Key] public int Id { get; set; }
    public int Number { get; set; }
    public DateOnly Date { get; set; }
    
    public int? SymbolId { get; set; }
    public DbSymbolDefinition? Symbol { get; set; }
}

public class DbSymbolDefinition
{
    [Key] public int Id { get; set; }
    public required string Symbol { get; set; }
    public required string Description { get; set; }
}

public class DbFriendConnection
{
    [Key] public int Id { get; set; }

    // The user who initiated the friend request
    public int User1Id { get; set; }
    public DbUser User1 { get; set; } = null!;

    // The user who received the friend request
    public int User2Id { get; set; }
    public DbUser User2 { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
    public bool IsAccepted { get; set; }
}