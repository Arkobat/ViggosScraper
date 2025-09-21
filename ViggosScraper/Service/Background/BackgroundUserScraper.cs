using DrikDatoApp.Service;
using Microsoft.EntityFrameworkCore;

namespace ViggosScraper.Service.Background;

public class BackgroundUserScraper(
    IServiceProvider serviceProvider,
    ILogger<BackgroundUserScraper> logger
) : BackgroundService
{
    private const int MinimumKnownUserId = 77364;
    private const int MaxConsecutiveNotFound = 250;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await WaitUntilMorning(stoppingToken);
            try
            {
                logger.LogInformation("Starting daily user scraping at {Time}", DateTime.Now);
                await ScrapeAllUsers(stoppingToken);
                logger.LogInformation("Completed daily user scraping at {Time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during user scraping");
            }
        }
    }

    private async Task ScrapeAllUsers(CancellationToken cancellationToken)
    {
        const int batchSize = 50;
        const int maxConcurrency = 10;

        var currentUserId = 1;
        var consecutiveNotFound = 0;
        var totalUsersFound = 0;
        var totalUsersProcessed = 0;

        logger.LogInformation("Starting user scan from ID 1, guaranteed to scan until at least ID {MinId}", MinimumKnownUserId);

        while (!cancellationToken.IsCancellationRequested)
        {
            var shouldContinue = currentUserId < MinimumKnownUserId || consecutiveNotFound < MaxConsecutiveNotFound;
            if (!shouldContinue) break;

            // Create a batch of user IDs to process
            var userIdBatch = Enumerable.Range(currentUserId, batchSize).ToList();

            // Process the batch with limited concurrency
            var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
            var tasks = userIdBatch.Select(async userId =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var userScraper = scope.ServiceProvider.GetRequiredService<UserScraper>();

                    var result = await userScraper.ScrapeUser(userId);

                    if (result != null)
                    {
                        logger.LogDebug("Successfully processed user {UserId}", userId);
                        return new { UserId = userId, Found = true, Error = (Exception?)null };
                    }
                    else
                    {
                        logger.LogDebug("User {UserId} not found", userId);
                        return new { UserId = userId, Found = false, Error = (Exception?)null };
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error processing user {UserId}", userId);
                    return new { UserId = userId, Found = false, Error = (Exception?)ex };
                }
                finally
                {
                    semaphore.Release();
                }
            });

            var results = await Task.WhenAll(tasks);

            // Process results and update counters
            var foundInBatch = 0;
            var notFoundInBatch = 0;

            foreach (var result in results.OrderBy(r => r.UserId))
            {
                if (result.Found)
                {
                    foundInBatch++;
                    consecutiveNotFound = 0;
                }
                else
                {
                    notFoundInBatch++;
                }

                totalUsersProcessed++;
            }

            totalUsersFound += foundInBatch;

            // Update consecutive not found counter based on the end of the batch
            if (notFoundInBatch == batchSize)
            {
                consecutiveNotFound += batchSize;
            }
            else if (foundInBatch > 0)
            {
                // Reset if we found any users in this batch
                consecutiveNotFound = 0;
            }
            else
            {
                // Some found, some not - count consecutive from the end
                var lastResults = results.OrderBy(r => r.UserId).TakeLast(MaxConsecutiveNotFound).ToList();
                consecutiveNotFound = lastResults.AsEnumerable().Reverse().TakeWhile(r => !r.Found).Count();
            }

            if (totalUsersProcessed % 1000 == 0)
            {
                var status = currentUserId < MinimumKnownUserId ? "scanning to minimum" : "checking for additional users";
                logger.LogInformation("Progress: Processed {Processed} users, found {Found} users, current ID: {CurrentId}, consecutive not found: {NotFound} ({Status})",
                    totalUsersProcessed, totalUsersFound, currentUserId, consecutiveNotFound, status);
            }

            currentUserId += batchSize;

            // Small delay between batches to avoid overwhelming the API
            await Task.Delay(200, cancellationToken);
        }

        logger.LogInformation("User scraping completed. Total users found: {TotalFound}, Total processed: {TotalProcessed}, Stopped at ID: {StoppedAt}",
            totalUsersFound, totalUsersProcessed, currentUserId);
    }


    private async Task WaitUntilMorning(CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var targetTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);

        if (now >= targetTime)
        {
            targetTime = targetTime.AddDays(1);
        }

        var delay = targetTime - now;

        if (delay.TotalMilliseconds > 0)
        {
            logger.LogInformation("Waiting until {TargetTime} to start user scraping (in {Delay})", targetTime, delay);
            await Task.Delay(delay, cancellationToken);
        }
    }
}