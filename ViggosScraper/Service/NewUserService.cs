using Microsoft.EntityFrameworkCore;
using ViggosScraper.Database;
using ViggosScraper.Model.Response;

namespace ViggosScraper.Service;

public class NewUserService(ViggosDb dbContext)
{
    public async Task<List<HighscoreEntry>> Highscore(int? year = null)
    {
        List<HighscoreEntry> results;

        if (year.HasValue)
        {
            // For a specific year, use actual date data we have
            results = await dbContext.Users
                .Select(u => new HighscoreEntry
                {
                    ProfileId = u.ProfileId.ToString(),
                    Name = u.Name,
                    Position = 0, // Will be calculated below
                    TotalDates = u.Datoer.Count(d => d.Date.Year == year.Value)
                })
                .Where(h => h.TotalDates > 0) // Only include users with dates in that year
                .OrderByDescending(h => h.TotalDates)
                .Take(150)
                .ToListAsync();
        }
        else
        {
            // For total/all-time, use TotalDatoer property
            results = await dbContext.Users
                .Select(u => new HighscoreEntry
                {
                    ProfileId = u.ProfileId.ToString(),
                    Name = u.Name,
                    Position = 0, // Will be calculated below
                    TotalDates = u.TotalDatoer
                })
                .Where(h => h.TotalDates > 0) // Only include users with dates
                .OrderByDescending(h => h.TotalDates)
                .Take(150)
                .ToListAsync();
        }

        // Calculate positions
        for (var i = 0; i < results.Count; i++)
        {
            results[i].Position = i + 1;
        }

        return results;
    }

    public async Task<List<SearchResult>> SearchUsers(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return [];
        }

        searchTerm = searchTerm.Trim();

        var users = await dbContext.Users
            .Where(u =>
                EF.Functions.ILike(u.Name, $"%{searchTerm}%") ||
                (u.RealName != null && EF.Functions.ILike(u.RealName, $"%{searchTerm}%")) ||
                (u.Glass != null && u.Glass == searchTerm) ||
                (u.Phone != null && u.Phone == searchTerm)
            )
            .Select(u => new SearchResult
            {
                ProfileId = u.ProfileId.ToString(),
                Name = u.Name,
            })
            .Take(100)
            .ToListAsync();

        return users;
    }
}