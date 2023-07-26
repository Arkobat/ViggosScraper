using ViggosScraper.Model;

namespace ViggosScraper.Service;

public class SymbolService
{
    private readonly List<DatoSymbol> _symbols = new()
    {
        new DatoSymbol {Date = new DateOnly(2023, 7, 23), Symbol = "Cykel", Reason = "Tour de France"},
        new DatoSymbol {Date = new DateOnly(2023, 7, 18), Symbol = "Cykel", Reason = "Tour de France"},
        new DatoSymbol {Date = new DateOnly(2023, 7, 15), Symbol = "Sol", Reason = "Morgendato"},
        new DatoSymbol {Date = new DateOnly(2023, 7, 14), Symbol = "Cykel", Reason = "Tour de France"},
        new DatoSymbol {Date = new DateOnly(2023, 7, 6), Symbol = "Cykel", Reason = "Tour de France"},
        new DatoSymbol {Date = new DateOnly(2023, 7, 1), Symbol = "Cykel", Reason = "Tour de France"},
        new DatoSymbol {Date = new DateOnly(2023, 4, 18), Symbol = "Flag", Reason = "Viggos Fødselsdag"},
        new DatoSymbol {Date = new DateOnly(2023, 3, 17), Symbol = "Firkløver", Reason = "Skt. Patricks Dag"},
        new DatoSymbol {Date = new DateOnly(2023, 2, 14), Symbol = "Hjerte", Reason = "Valentines Dag"},

        new DatoSymbol {Date = new DateOnly(2022, 12, 18), Symbol = "Adventslys", Reason = "Advent #4"},
        new DatoSymbol {Date = new DateOnly(2022, 12, 11), Symbol = "Adventslys", Reason = "Advent #3"},
        new DatoSymbol {Date = new DateOnly(2022, 12, 4), Symbol = "Adventslys", Reason = "Advent #2"},
        new DatoSymbol {Date = new DateOnly(2022, 11, 27), Symbol = "Adventslys", Reason = "Advent #1"},
        new DatoSymbol {Date = new DateOnly(2022, 3, 31), Symbol = "Flag", Reason = "Viggos Fødselsdag"},
        new DatoSymbol {Date = new DateOnly(2022, 3, 17), Symbol = "Firkløver", Reason = "Skt. Patricks Dag"},
        new DatoSymbol {Date = new DateOnly(2022, 2, 22), Symbol = "2", Reason = "2 Tals Dato"},
        new DatoSymbol {Date = new DateOnly(2022, 2, 14), Symbol = "Hjerte", Reason = "Valentine"},

        new DatoSymbol {Date = new DateOnly(2021, 12, 19), Symbol = "Adventslys", Reason = "Advent #4"},
        new DatoSymbol {Date = new DateOnly(2021, 12, 12), Symbol = "Adventslys", Reason = "Advent #3"},
        new DatoSymbol {Date = new DateOnly(2021, 12, 5), Symbol = "Adventslys", Reason = "Advent #2"},
        new DatoSymbol {Date = new DateOnly(2021, 11, 28), Symbol = "Adventslys", Reason = "Advent #1"},
        new DatoSymbol {Date = new DateOnly(2021, 4, 24), Symbol = "Corona", Reason = "Tillids Dato"},

        new DatoSymbol {Date = new DateOnly(2020, 12, 6), Symbol = "Adventslys", Reason = "Advent #2"},
        new DatoSymbol {Date = new DateOnly(2020, 11, 29), Symbol = "Adventslys", Reason = "Advent #1"},
        new DatoSymbol {Date = new DateOnly(2020, 9, 5), Symbol = "Krus", Reason = "Øllets Dag"},
        new DatoSymbol {Date = new DateOnly(2020, 4, 24), Symbol = "Corona", Reason = "Tillids Dato"},
        new DatoSymbol {Date = new DateOnly(2020, 2, 29), Symbol = "Stjerneskud ", Reason = "Skudår"},
        new DatoSymbol {Date = new DateOnly(2020, 2, 14), Symbol = "Hjerte", Reason = "Valtentine"},

        new DatoSymbol {Date = new DateOnly(2019, 12, 22), Symbol = "Adventslys", Reason = "Advent #4"},
        new DatoSymbol {Date = new DateOnly(2019, 12, 15), Symbol = "Adventslys", Reason = "Advent #3"},
        new DatoSymbol {Date = new DateOnly(2019, 12, 8), Symbol = "Adventslys", Reason = "Advent #2"},
        new DatoSymbol {Date = new DateOnly(2019, 12, 1), Symbol = "Adventslys", Reason = "Advent #1"},
        new DatoSymbol {Date = new DateOnly(2019, 11, 1), Symbol = "Sol", Reason = "J Dag"},
        new DatoSymbol {Date = new DateOnly(2019, 9, 7), Symbol = "Krus", Reason = "Øllets Dag"},
        new DatoSymbol {Date = new DateOnly(2019, 7, 13), Symbol = "Sol", Reason = "Morgendato"},
        new DatoSymbol {Date = new DateOnly(2019, 3, 27), Symbol = "Flag", Reason = "Viggos Fødselsdag"},
        new DatoSymbol {Date = new DateOnly(2019, 3, 17), Symbol = "Firkløver", Reason = "Skt. Patricks Dag"},
        new DatoSymbol {Date = new DateOnly(2019, 2, 14), Symbol = "Hjerte", Reason = "Valentine"},
    };


    public List<DatoSymbol> GetSymbols(List<DateOnly> dates)
    {
        return _symbols
            .Where(s => dates.Contains(s.Date))
            .ToList();
    }

    public List<DatoSymbol> GetAll() => _symbols.ToList();
}