using SmartLearning.DTOs;
using SmartLearning.Repositories;

namespace SmartLearning.Services;

public class StatisticService(
    IReviewRepository reviewRepo,
    ITransactionRepository transactionRepo
    ) : IStatisticService
{
    public async Task<StatisticDto> GetStatisticsAsync(string userId)
    {
        var streak = await GetStreakDataAsync(userId);
        var xp = await transactionRepo.GetXpStatistics(userId);
        var dailyReviews = await reviewRepo.GetDailyReviewDataAsync(userId, 30);

        return new StatisticDto
        {
            StreakData = streak,
            XpData = xp,
            DailyReviewData = dailyReviews
        };
    }
    public async Task<StreakData> GetStreakDataAsync(string userId)
    {
        if (userId is null)
            throw new KeyNotFoundException("User not found");
        
        var dates = await reviewRepo.GetDistinctReviewDatesAsync(userId);
        
        var (longest, current) = CalculateStreaks(dates);

        return new StreakData
        {
            CurrentStreak = current,
            LongestStreak = longest,
            ReviewDates = dates.ToList(),
        };
    }

    private (int Longest, int Current) CalculateStreaks(HashSet<DateOnly> dates)
    {
        if (dates.Count == 0)
            return (0, 0);
        
        var longestStreak = 0;
        
        foreach (var date in dates)
        {
            if (dates.Contains(date.AddDays(-1))) continue;
            var length = 1;
            var current = date;

            while (dates.Contains(current.AddDays(1)))
            {
                current = current.AddDays(1);
                length++;
            }

            longestStreak = Math.Max(longestStreak, length);
        }

        var currentStreak = 0;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var startDate = dates.Contains(today)
            ? today
            : dates.Contains(today.AddDays(-1))
                ? today.AddDays(-1)
                : (DateOnly?)null;

        if (startDate.HasValue)
        {
            var current = startDate.Value;

            while (dates.Contains(current))
            {
                currentStreak++;
                current = current.AddDays(-1);
            }
        }

        return (longestStreak, currentStreak);
    } 
}