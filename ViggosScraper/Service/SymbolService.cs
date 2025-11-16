using Microsoft.EntityFrameworkCore;
using ViggosScraper.Database;
using ViggosScraper.Model.Response;

namespace ViggosScraper.Service;

public class SymbolService
{
    private readonly ViggosDb _dbContext;

    public SymbolService(ViggosDb dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<DbLogoGroup>> GetLogos(List<DateOnly> dates, List<string> permissions)
    {
        var result = await _dbContext.LogosDates
            .Where(l => (dates.Contains(l.Date) && (l.Permission == null)) || permissions.Contains(l.Permission!))
            .Include(d => d.Group)
            .ToListAsync();
        
        return result
            .Select(r => r.Group)
            .DistinctBy(l => l.Symbol)
            .ToList();
    }

    public async Task<List<DbLogoGroup>> GetAll()
    {
        return await _dbContext.LogoGroups
            .Include(l => l.Dates)
            .ToListAsync();
    }

    public async Task Seed()
    {
        if (await _dbContext.LogoGroups.AnyAsync()) throw new Exception("Already seeded");
         // @formatter:off
        var defaultData = new List<DbLogoGroup>
        {
            new() {Symbol = "Cykel",  Dates = new List<DbLogo> {
                new() {Description = "Tour de France: 1. etape", Date = new DateOnly(2023, 07, 01)},
                new() {Description = "Tour de France: 6. etape", Date = new DateOnly(2023, 07, 06)},
                new() {Description = "Tour de France: 13. etape", Date = new DateOnly(2023, 07, 14)},
                new() {Description = "Tour de France: 16. etape", Date = new DateOnly(2023, 07, 18)},
                new() {Description = "Tour de France: 21. etape", Date = new DateOnly(2023, 07, 23)},
                new() {Description = "Tour de France: 2. etape", Date = new DateOnly(2024, 07, 02)},
                new() {Description = "Tour de France: 7. etape", Date = new DateOnly(2024, 07, 07)},
                new() {Description = "Tour de France: 14. etape", Date = new DateOnly(2024, 07, 14)},
                new() {Description = "Tour de France: 19. etape", Date = new DateOnly(2024, 07, 19)},
                new() {Description = "Tour de France: 21. etape", Date = new DateOnly(2024, 07, 21)},
            }},
            new() {Symbol = "Sol",  Dates = new List<DbLogo> {
                new() {Description = "Morgendato", Date = new DateOnly(2019, 07, 13)},
                new() {Description = "Morgendato", Date = new DateOnly(2023, 07, 15)},
                new() {Description = "FC Campus Morgendato", Date = new DateOnly(2023, 02, 04), Permission = "fccampus-181123"},
                new() {Description = "Morgendato", Date = new DateOnly(2024, 09, 07)},
            }},
            new() {Symbol = "Flag",  Dates = new List<DbLogo> {
                new() {Description = "Viggos Fødselsdag", Date = new DateOnly(2019, 03, 27)},
                new() {Description = "Viggos Fødselsdag", Date = new DateOnly(2022, 03, 31)},
                new() {Description = "Viggos Fødselsdag", Date = new DateOnly(2023, 04, 18)},
                new() {Description = "Viggos Fødselsdag", Date = new DateOnly(2024, 04, 23)},
            }},
            new() {Symbol = "Firkløver",  Dates = new List<DbLogo> {
                new() {Description = "Skt. Patricks Dag", Date = new DateOnly(2019, 03, 17)},
                new() {Description = "Skt. Patricks Dag", Date = new DateOnly(2022, 03, 17)},
                new() {Description = "Skt. Patricks Dag", Date = new DateOnly(2023, 03, 17)},
                new() {Description = "Skt. Patricks Dag", Date = new DateOnly(2024, 03, 17)},
                new() {Description = "Skt. Patricks Dag", Date = new DateOnly(2025, 03, 17)},
            }},
            new() {Symbol = "Hjerte", Dates = new List<DbLogo> {
                new() {Description = "Valentine", Date = new DateOnly(2019, 02, 14)},
                new() {Description = "Valentine", Date = new DateOnly(2020, 02, 14)},
                new() {Description = "Valentine", Date = new DateOnly(2022, 02, 14)},
                new() {Description = "Valentine", Date = new DateOnly(2023, 02, 14)},
                new() {Description = "Valentine", Date = new DateOnly(2024, 02, 14)},
                new() {Description = "Valentine", Date = new DateOnly(2025, 02, 14)},
            }},
            new() {Symbol = "Adventslys", Dates = new List<DbLogo> {
                new() {Description = "Advent #1", Date = new DateOnly(2019, 12, 01)},
                new() {Description = "Advent #2", Date = new DateOnly(2019, 12, 08)},
                new() {Description = "Advent #3", Date = new DateOnly(2019, 12, 15)},
                new() {Description = "Advent #4", Date = new DateOnly(2019, 12, 22)},
                new() {Description = "Advent #1", Date = new DateOnly(2020, 11, 29)},
                new() {Description = "Advent #2", Date = new DateOnly(2020, 12, 06)},
                new() {Description = "Advent #1", Date = new DateOnly(2021, 11, 28)},
                new() {Description = "Advent #2", Date = new DateOnly(2021, 12, 05)},
                new() {Description = "Advent #3", Date = new DateOnly(2021, 12, 12)},
                new() {Description = "Advent #4", Date = new DateOnly(2021, 12, 19)},
                new() {Description = "Advent #1", Date = new DateOnly(2022, 11, 27)},
                new() {Description = "Advent #2", Date = new DateOnly(2022, 12, 04)},
                new() {Description = "Advent #3", Date = new DateOnly(2022, 12, 11)},
                new() {Description = "Advent #4", Date = new DateOnly(2022, 12, 18)},
                new() {Description = "Advent #1", Date = new DateOnly(2023, 12, 03)},
                new() {Description = "Advent #2", Date = new DateOnly(2023, 12, 10)},
                new() {Description = "Advent #3", Date = new DateOnly(2023, 12, 17)},
                new() {Description = "Advent #4", Date = new DateOnly(2023, 12, 24)},
                new() {Description = "Advent #1", Date = new DateOnly(2024, 11, 01)},
                new() {Description = "Advent #2", Date = new DateOnly(2024, 12, 08)},
                new() {Description = "Advent #3", Date = new DateOnly(2024, 12, 15)},
                new() {Description = "Advent #4", Date = new DateOnly(2024, 12, 22)},
                new() {Description = "Advent #1", Date = new DateOnly(2025, 11, 30)},
                new() {Description = "Advent #2", Date = new DateOnly(2025, 12, 7)},
                new() {Description = "Advent #3", Date = new DateOnly(2025, 12, 14)},
                new() {Description = "Advent #4", Date = new DateOnly(2025, 12, 21)},
            }},
            new() {Symbol = "2",  Dates = new List<DbLogo> {
                new() {Description = "2 Tals Dato", Date = new DateOnly(2022, 02, 22)},
            }},
            new() {Symbol = "Nissehue",  Dates = new List<DbLogo> {
                new() {Description = "J-dag", Date = new DateOnly(2019, 11, 01)},
                new() {Description = "J-dag", Date = new DateOnly(2023, 11, 03)},
            }},
            new() {Symbol = "Corona",  Dates = new List<DbLogo> {
                new() {Description = "Tillids Dato #1", Date = new DateOnly(2020, 04, 24)},
                new() {Description = "Tillids Dato #2", Date = new DateOnly(2021, 04, 24)},
            }},
            new() {Symbol = "Krus",  Dates = new List<DbLogo> {
                new() {Description = "Øllets Dag", Date = new DateOnly(2019, 09, 07)},
                new() {Description = "Øllets Dag", Date = new DateOnly(2020, 09, 05)},
                new() {Description = "Oktoberfest", Date = new DateOnly(2024, 11, 26)},
            }},
            new() {Symbol = "Stjerneskud",  Dates = new List<DbLogo> {
                new() {Description = "Skudår", Date = new DateOnly(2020, 02, 29)},
                new() {Description = "Skudår", Date = new DateOnly(2024, 02, 29)},
            }},
            new() {Symbol = "Node",  Dates = new List<DbLogo> {
                new() {Description = "Fællessang", Date = new DateOnly(2025, 02, 11)},
            }},
            
            // Private dates, which are only for users attending a specific event
            new() {Symbol = "Fodbold",  Dates = new List<DbLogo> {
                new() {Description = "Fodbold", Date = new DateOnly(2023, 02, 04), Permission = "fccampus-040223"},
            }},
            new() {Symbol = "KJ",  Dates = new List<DbLogo> {
                new() {Description = "Kristian 27 år", Date = new DateOnly(2024, 1, 25), Permission = "kj-250124"},
            }},
            new() {Symbol = "Dollar",  Dates = new List<DbLogo> {
                new() {Description = "Sparekassen", Date = new DateOnly(2019, 11, 16), Permission = "sparekassen-161119"},
                new() {Description = "Sparekassen", Date = new DateOnly(2021, 11, 27), Permission = "sparekassen-271121"},
                new() {Description = "Sparekassen", Date = new DateOnly(2022, 11, 16), Permission = "sparekassen-161122"},
            }},
            new() {Symbol = "SIF",  Dates = new List<DbLogo> {
                new() {Description = "SIF Fraktalet", Date = new DateOnly(2024, 6, 8), Permission = "sif-080624"},
                new() {Description = "SIF Fraktalet", Date = new DateOnly(2024, 11, 9), Permission = "sif-091124"},
            }},
            new() {Symbol = "Enigma",  Dates = new List<DbLogo> {
                new() {Description = "Enigma", Date = new DateOnly(2024, 11, 16), Permission = "enigma-161124"},
            }},
            new() {Symbol = "Leo",  Dates = new List<DbLogo> {
                new() {Description = "Leo", Date = new DateOnly(2024, 7, 27), Permission = "leo-270724"},
            }},
            new() {Symbol = "Nico",  Dates = new List<DbLogo> {
                new() {Description = "Nicholas", Date = new DateOnly(2024, 8, 2), Permission = "nicolas-020824"},
            }},
            new() {Symbol = "E",  Dates = new List<DbLogo> {
                new() {Description = "Emilie", Date = new DateOnly(2023, 10, 19), Permission = "emilie-191023"},
                new() {Description = "Emilie", Date = new DateOnly(2024, 10, 19), Permission = "emilie-191024"},
            }},
        }; 
        // @formatter:on
        await _dbContext.LogoGroups.AddRangeAsync(defaultData);
        await _dbContext.SaveChangesAsync();
    }

    public static SimpleDatoSymbol? GetSymbol(List<DbLogoGroup> symbols, DateOnly date, DateTime? startDate)
    {
        var matchingGroup = symbols.FirstOrDefault(s => s.Dates.Any(logo => logo.Date == date));
        if (matchingGroup is not null)
        {
            var matchingLogo = matchingGroup.Dates.First(logo => logo.Date == date);
            return new SimpleDatoSymbol
            {
                Symbol = matchingGroup.Symbol,
                Reason = matchingLogo.Description
            };
        }
        
        if (IsDatoByNight(startDate))
        {
            return new SimpleDatoSymbol
            {
                Symbol = "Måne",
                Reason = "Dato by Night"
            };
        }
        
        return null;
    }

    private static bool IsDatoByNight(DateTime? startDate)
    {
        if (startDate is null)
        {
            return false;
        }

        return startDate.Value.DayOfWeek switch
        {
            DayOfWeek.Friday => startDate.Value.Hour is >= 20 or < 6,
            DayOfWeek.Saturday => startDate.Value.Hour is >= 20 or < 6,
            _ => false
        };
    }
}