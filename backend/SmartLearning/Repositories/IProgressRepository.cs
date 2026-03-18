using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface IProgressRepository 
{
    Task<UserCardProgress?> GetProgressAsync(string userId, Guid cardId);
    Task AddProgressAsync(UserCardProgress progress);

    Task SaveChangesAsync();
}