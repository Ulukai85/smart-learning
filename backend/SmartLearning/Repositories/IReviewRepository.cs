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