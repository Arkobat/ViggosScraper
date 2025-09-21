using DrikDatoApp.Model;
using DrikDatoApp.Service;
using Microsoft.EntityFrameworkCore;
using ViggosScraper.Database;

namespace ViggosScraper.Service;

public class UserScraper(
    IDrikDatoService drikDatoService,
    ViggosDb dbContext,
    ILogger<UserScraper> logger
)
{
    public async Task<DbUser?> ScrapeUser(int userId)
    {
        try
        {
            logger.LogDebug("Starting scrape for user {UserId}", userId);

            var userResponse = await drikDatoService.GetUser(userId.ToString());

            if (userResponse.Status != 1)
            {
                logger.LogInformation("User {UserId} not found", userId);
                return null;
            }

            logger.LogDebug("Found user {UserId}: {UserName} ({Alias})",
                userId, userResponse.Player!.Name, userResponse.Player.Alias);

            var user = await ProcessUser(userResponse.Player);
            logger.LogInformation("Successfully scraped user {UserId}", userId);
            return user;

        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning("HTTP error for user {UserId}: {Error}", userId, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error scraping user {UserId}", userId);
            throw;
        }
    }

    private async Task<DbUser> ProcessUser(UserDto player)
    {
        try
        {
            logger.LogTrace("Processing user {UserId}: {Name} ({Alias})", player.Id, player.Name, player.Alias);

            var existingUser = await dbContext.Users
                .Include(u => u.Datoer)
                .FirstOrDefaultAsync(u => u.ProfileId == player.Id);

            if (existingUser is null)
            {
                existingUser = await CreateNewUser(player);
            }
            else
            {
                existingUser.Name = player.Alias;
                existingUser.RealName = player.Name;
                existingUser.AvatarUrl = player.Photo;
                existingUser.Glass = player.GlassNumber;
                existingUser.LastUpdated = DateTimeOffset.UtcNow;
                existingUser.LastChecked = DateTimeOffset.UtcNow;
                await dbContext.SaveChangesAsync();
            }

            await UpdateUserDates(existingUser, player);
            await dbContext.SaveChangesAsync();

            logger.LogTrace("Successfully saved user {UserId} to database", player.Id);
            return existingUser;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process user {UserId}", player.Id);
            throw;
        }
    }


    private async Task<DbUser> CreateNewUser(UserDto player)
    {
        if (player.Token is not null)
        {
            var authorizeResponse = await drikDatoService.Authorize(player.Token);
            if (authorizeResponse.Status == 1)
            {
                return await CreateUser(authorizeResponse.Player!);
            }
        }

        return new DbUser
        {
            ProfileId = player.Id,
            Name = player.Alias,
            RealName = player.Name,
            AvatarUrl = player.Photo,
            //AvatarUrl = !string.IsNullOrEmpty(player.Photo) ? $"https://www.drikdato.app/uploads/{player.Photo}" : null,
            Glass = player.GlassNumber,
            Phone = player.Phone,
            TotalDatoer = 0,
            LastUpdated = DateTimeOffset.UtcNow,
            Datoer = []
        };
    }

    private async Task UpdateUserDates(DbUser user, UserDto player)
    {
        if (player.NumDates == user.TotalDatoer)
        {
            // No change in the number of dates, nothing to do
            return;
        }
        
        // Check if we have a valid token to fetch detailed date info
        if (!string.IsNullOrEmpty(player.Token))
        {
            var authorizeResponse = await drikDatoService.Authorize(player.Token);
            // If token is valid, update dates from detailed info
            if (authorizeResponse.Status == 1)
            {
                user.TotalDatoer = authorizeResponse.Player!.Dates.Count;

                user.Datoer.Clear();
                var dateNumber = player.NumDates;
                user.Datoer = authorizeResponse.Player.Dates.Select(d => new DbDato
                {
                    Number = dateNumber--,
                    Date = DateOnly.FromDateTime(DateFormatter.MapViggosDate(d.DateFormatted)),
                    StartDate = DateFormatter.MapViggosDate(d.DateFormatted),
                    EndDate = DateFormatter.MapViggosDate(d.EndDateFormatted),
                    Comment = d.Comment,
                }).ToList();
                return;
            }
        }
        
        // Check if the number of dates has changed since we last checked
        var now = DateTimeOffset.UtcNow;
        var yesterday = now.AddDays(-1).Date;
        var wasCheckedYesterday = user.LastChecked.HasValue &&
                                  user.LastChecked.Value.Date == yesterday;
        
        // If we checked them yesterday, and they have exactly one more date than before, we can infer they got a new date yesterday
        if (wasCheckedYesterday && player.NumDates == user.TotalDatoer + 1)
        {
            user.Datoer.Add(new DbDato
            {
                Number = player.NumDates,
                Date = new DateOnly(yesterday.Year, yesterday.Month, yesterday.Day),
                StartDate = null,
                EndDate = null,
                Comment = null
            });
        }
        
        // Always update the total date count to match the current number
        user.TotalDatoer = player.NumDates;
    }

    public async Task<DbUser> CreateUser(SelfUserDto player)
    {
        var dateNumber = player.Dates.Count;
        var dbUser = new DbUser
        {
            ProfileId = player.Id,
            Name = player.Alias,
            RealName = player.Name,
            AvatarUrl = player.Photo,
            Glass = player.GlassNumber,
            Phone = player.Phone,
            TotalDatoer = player.Dates.Count,
            Datoer = player.Dates.Select(d => new DbDato
            {
                Number = dateNumber--,
                Date = DateOnly.FromDateTime(DateFormatter.MapViggosDate(d.DateFormatted)),
                StartDate = DateFormatter.MapViggosDate(d.DateFormatted),
                EndDate = DateFormatter.MapViggosDate(d.EndDateFormatted),
                Comment = d.Comment,
            }).ToList(),
            Permissions = [],
            LastUpdated = DateTimeOffset.UtcNow,
            LastChecked = DateTimeOffset.UtcNow,
        };
        dbContext.Users.Add(dbUser);
        await dbContext.SaveChangesAsync();

        return dbUser;
    }

    public async Task UpdateUser(DbUser user, SelfUserDto player)
    {
        user.Name = player.Alias;
        user.RealName = player.Name;
        user.AvatarUrl = player.Photo;
        user.Glass = player.GlassNumber;
        user.LastUpdated = DateTimeOffset.UtcNow;
        user.LastChecked = DateTimeOffset.UtcNow;

        if (user.TotalDatoer != player.Dates.Count)
        {
            user.TotalDatoer = player.Dates.Count;

            // Clear existing dates and add all from the API to ensure we have the complete set
            user.Datoer.Clear();
            var dateNumber = player.Dates.Count;
            user.Datoer = player.Dates.Select(d => new DbDato
            {
                Number = dateNumber--,
                Date = DateOnly.FromDateTime(DateFormatter.MapViggosDate(d.DateFormatted)),
                StartDate = DateFormatter.MapViggosDate(d.DateFormatted),
                EndDate = DateFormatter.MapViggosDate(d.EndDateFormatted),
                Comment = d.Comment,
            }).ToList();
        }

        await dbContext.SaveChangesAsync();
    }
}