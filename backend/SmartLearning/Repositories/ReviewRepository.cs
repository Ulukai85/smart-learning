using Microsoft.EntityFrameworkCore;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface IReviewRepository 
{
    Task<ReviewLog>SaveReviewLog(ReviewLog log);
    Task<UserCardProgress?> GetUserCardProgress(string userId, Guid cardId);
    Task<UserCardProgress> CreateUserCardProgress(UserCardProgress progress);
    Task SaveChangesAsync();
}

public class ReviewRepository(AppDbContext dbContext) : IReviewRepository
{
    public async Task<ReviewLog> SaveReviewLog(ReviewLog log)
    {
        await dbContext.ReviewLog.AddAsync(log);
        await dbContext.SaveChangesAsync();

        return log;
    }

    public async Task<UserCardProgress> CreateUserCardProgress(UserCardProgress progress)
    {
        await dbContext.UserCardProgresses.AddAsync(progress);
        await dbContext.SaveChangesAsync();

        return progress;
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
    
    public async Task<UserCardProgress?> GetUserCardProgress(string userId, Guid cardId)
    {
        var progress = await dbContext.UserCardProgresses
            .Where(p => p.UserId == userId && p.CardId == cardId)
            .FirstAsync();
        
        return progress;
    }
}