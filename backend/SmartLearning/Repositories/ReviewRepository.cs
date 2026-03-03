using Microsoft.EntityFrameworkCore;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface IReviewRepository 
{
    Task AddReviewLogAsync(ReviewLog log);
    Task SaveChangesAsync();
    Task<HashSet<DateOnly>> GetDistinctReviewDatesAsync(string userId);
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
}