﻿using Microsoft.EntityFrameworkCore;
using ViggosScraper.Database;
using ViggosScraper.Model;

namespace ViggosScraper.Service;

public class UserService
{
    private readonly ViggosDb _dbContext;
    private readonly UserScraper _userScraper;
    private readonly ILogger<UserService> _logger;

    public UserService(ViggosDb dbContext, UserScraper userScraper, ILogger<UserService> logger)
    {
        _dbContext = dbContext;
        _userScraper = userScraper;
        _logger = logger;
    }

    public async Task<DbUser> GetUser(string userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.ProfileId == userId);

        if (user is null || user.ShouldUpdate())
        {
            _logger.LogInformation("Scraping user {UserId}", userId);
            var scrapedUser = await _userScraper.GetUser(user, userId);
            user = await UpsertUser(scrapedUser, user);
            await _dbContext.SaveChangesAsync();
        }

        return user;
    }

    public async Task<DbUser> UpsertUser(UserDto scrapedUser, DbUser? dbUser)
    {
        if (dbUser is null)
        {
            dbUser = new DbUser()
            {
                ProfileId = scrapedUser.ProfileId,
                Name = scrapedUser.Name,
                RealName = scrapedUser.RealName,
                AvatarUrl = scrapedUser.AvatarUrl,
                Glass = scrapedUser.Krus,
                LastUpdated = DateTimeOffset.UtcNow,
                Datoer = scrapedUser.Dates.Select(d => new DbDato()
                {
                    Number = d.Number,
                    Date = d.Date,
                }).ToList()
            };
            await _dbContext.Users.AddAsync(dbUser);
            await _dbContext.SaveChangesAsync();
            return dbUser;
        }

        dbUser.Name = scrapedUser.Name;
        if (scrapedUser.RealName is not null) dbUser.RealName = scrapedUser.RealName;
        dbUser.AvatarUrl = scrapedUser.AvatarUrl;
        dbUser.Glass = scrapedUser.Krus;
        dbUser.LastUpdated = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();
        return dbUser;
    }
}