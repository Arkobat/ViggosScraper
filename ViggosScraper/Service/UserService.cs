using Microsoft.EntityFrameworkCore;
using ViggosScraper.Database;
using ViggosScraper.Model;

namespace ViggosScraper.Service;

public class UserService
{
    private readonly UserScraper _userScraper;
    private readonly ViggosDbContext _dbContext;

    public UserService(UserScraper userScraper, ViggosDbContext dbContext)
    {
        _userScraper = userScraper;
        _dbContext = dbContext;
    }

    public async Task<List<DbUser>> GetDateInfo(DateOnly date, string userId)
    {
        var user = await GetUser(userId);
        var friendIds = new List<int>();
        friendIds.AddRange(user.Friends.Select(f => f.User1Id));
        friendIds.AddRange(user.Friends.Select(f => f.User2Id));
        friendIds.Remove(user.Id);

        return await _dbContext.Users
            .Where(f => friendIds.Contains(f.Id))
            .ToListAsync();
    }

    public async Task<DbUser> GetUser(string userId)
    {
        var existingUser = await _dbContext.Users
            .Include(f => f.Dates).ThenInclude(d => d.Symbol)
            .Include(f => f.Friends)
            .FirstOrDefaultAsync(u => u.ProfileId == userId);

        if (existingUser is not null && existingUser.LastUpdated + TimeSpan.FromHours(3) > DateTimeOffset.Now)
            return existingUser;


        var user = await _userScraper.GetUser(userId);
        return await UpsertUser(existingUser, user);
    }

    private async Task<DbUser> UpsertUser(DbUser? dbUser, User viggosUser)
    {
        // Create new user if it doesn't exist
        if (dbUser is null)
        {
            dbUser = new DbUser
            {
                ProfileId = viggosUser.ProfileId,
                Name = viggosUser.Name,
                AvatarUrl = viggosUser.AvatarUrl,
                Krus = viggosUser.Krus,
                LastUpdated = DateTimeOffset.UtcNow,
                Dates = viggosUser.Dates.Select(d => new DbDato
                {
                    Date = d.Date,
                    Number = d.Number
                }).ToList()
            };
            await _dbContext.Users.AddAsync(dbUser);
            return dbUser;
        }

        // Update existing user
        dbUser.LastUpdated = DateTimeOffset.UtcNow;
        dbUser.AvatarUrl = viggosUser.AvatarUrl;
        dbUser.Dates = viggosUser.Dates.Select(d => new DbDato
        {
            Date = d.Date,
            Number = d.Number
        }).ToList();
        await _dbContext.SaveChangesAsync();
        return dbUser;
    }
}