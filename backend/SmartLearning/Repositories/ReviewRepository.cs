using Microsoft.EntityFrameworkCore;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface IReviewRepository 
{
    Task AddReviewLogAsync(ReviewLog log);
    Task SaveChangesAsync();
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
}