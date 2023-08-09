using Microsoft.EntityFrameworkCore;
using ViggosScraper.Database;

namespace ViggosScraper.Service;

public class BackgroundUserScraper : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;


    public static readonly TimeSpan UpdateInterval = TimeSpan.FromHours(12);

    public BackgroundUserScraper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = _serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<BackgroundUserScraper>>();
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        while (true)
        {
            var lastRun = DateTimeOffset.UtcNow;

            try
            {
                logger.LogInformation("Starting to scrap users");
                await ScrapeUsers(lastRun, scope, stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while scraping users");
            }

            // Calculate when to run again
            var delay = (lastRun + UpdateInterval) - DateTimeOffset.Now;
            if (delay > TimeSpan.Zero)
            {
                logger.LogInformation("Waiting {Delay} before scraping users again", delay);
                await Task.Delay(delay, stoppingToken);
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private async Task ScrapeUsers(DateTimeOffset lastRun, IServiceScope scope, CancellationToken stoppingToken)
    {

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<BackgroundUserScraper>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ViggosDb>();
        var userScraper = scope.ServiceProvider.GetRequiredService<UserScraper>();
        var userService = scope.ServiceProvider.GetRequiredService<UserService>();


        List<DbUser>? usersToLoad = null;

        // Update all users that have not been updated in the last 12 hours
        while (usersToLoad == null || usersToLoad.Count > 0)
        {
            usersToLoad = await dbContext.Users
                .Where(u => u.LastUpdated < lastRun - UpdateInterval)
                .OrderBy(u => u.LastUpdated)
                .Take(10)
                .ToListAsync(cancellationToken: stoppingToken);

            foreach (var user in usersToLoad)
            {
                logger.LogInformation("Updating user {UserId}", user.ProfileId);
                var scrapedUser = await userScraper.GetUser(user.ProfileId);
                await userService.UpsertUser(scrapedUser, user);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}