using System.ComponentModel.DataAnnotations;
using ViggosScraper.Model;
using ViggosScraper.Model.Response;

namespace ViggosScraper.Database;

public class DbLogoGroup
{
    [Key] public int Id { get; set; }
    public required string Symbol { get; set; }
    public List<DbLogo> Dates { get; set; } = null!;
}

public class DbLogo
{
    [Key] public int Id { get; set; }
    public required string Description { get; set; }
    public required DateOnly Date { get; set; }
    public bool Private { get; set; } = false;

    public int GroupId { get; set; }
    public DbLogoGroup Group { get; set; } = null!;

    public SimpleDatoSymbol ToDto()
    {
        return new SimpleDatoSymbol
        {
            Symbol = Group.Symbol,
            Reason = Description
        };
    }
}