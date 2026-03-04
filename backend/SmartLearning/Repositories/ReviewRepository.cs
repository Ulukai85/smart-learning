using System.Runtime.InteropServices.JavaScript;
using Microsoft.EntityFrameworkCore;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface IReviewRepository 
{
    Task AddReviewLogAsync(ReviewLog log);
    Task SaveChangesAsync();
    Task<HashSet<DateOnly>> GetDistinctReviewDatesAsync(string userId);
    Task<List<DailyReviewDto>> GetDailyReviewDataAsync(string userId, int dateRange = 30);
}

public class ReviewRepository(AppDbContext dbContext) : IReviewRepository
{
    public async Task AddReviewLogAsync(ReviewLog log)
    {    
        await dbContext.ReviewLog.AddAsync(log);
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task<HashSet<DateOnly>> GetDistinctReviewDatesAsync(string userId)
    {
        var reviewDates = await dbContext.ReviewLog
            .Where(r => r.UserId == userId)
            .Select(r => r.ReviewedAt)
            .Distinct()
            .ToListAsync();

        return reviewDates.Select(DateOnly.FromDateTime).ToHashSet();
    }

    public async Task<List<DailyReviewDto>> GetDailyReviewDataAsync(string userId, int dateRange = 30)
    {
        var grouped = await dbContext.ReviewLog
            .Where(r => r.UserId == userId)
            .GroupBy(r => r.ReviewedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                CardsReviewed = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();
        
        var groupedDict = grouped.ToDictionary(g => g.Date, g => g.CardsReviewed);
        var today = DateTime.UtcNow.Date;
        var fromDate = today.AddDays(-dateRange);
        
        var result = new List<DailyReviewDto>();
        
        for (var date = fromDate; date <= today; date = date.AddDays(1))
        {
            groupedDict.TryGetValue(date, out var cardsReviewed);
            result.Add(new DailyReviewDto
            {
                Date = DateOnly.FromDateTime(date),
                CardsReviewed = cardsReviewed,
            });
        }

        return result;
    }
}