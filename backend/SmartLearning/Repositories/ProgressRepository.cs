using Microsoft.EntityFrameworkCore;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface IProgressRepository 
{
    Task<UserCardProgress?> GetProgressAsync(string userId, Guid cardId);
    Task AddProgressAsync(UserCardProgress progress);

    Task SaveChangesAsync();
}

public class ProgressRepository(AppDbContext dbContext) : IProgressRepository
{
    public async Task<UserCardProgress?> GetProgressAsync(string userId, Guid cardId)
    {
        var progress = await dbContext.UserCardProgresses
            .Where(p => p.UserId == userId && p.CardId == cardId)
            .FirstOrDefaultAsync();
        
        return progress;
    }

    public async Task AddProgressAsync(UserCardProgress progress)
    {
        await dbContext.UserCardProgresses.AddAsync(progress);
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}