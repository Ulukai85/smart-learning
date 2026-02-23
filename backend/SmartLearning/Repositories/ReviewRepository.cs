using Microsoft.EntityFrameworkCore;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface IReviewRepository 
{
    Task<UserCardProgress?> GetUserCardProgress(string userId, Guid cardId);
}

public class ReviewRepository(AppDbContext dbContext) : IReviewRepository
{
    public async Task<UserCardProgress?> GetUserCardProgress(string userId, Guid cardId)
    {
        var progress = await dbContext.UserCardProgresses
            .Where(p => p.UserId == userId && p.CardId == cardId)
            .FirstOrDefaultAsync();
        
        return progress;
    }
}