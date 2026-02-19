using SmartLearning.DTOs;
using SmartLearning.Repositories;

namespace SmartLearning.Services;

public interface IReviewService
{
    Task<ICollection<CardToReviewDto>> GetCardsToReviewAsync(
        Guid deckId,
        string userId,
        int dueLimit,
        int newLimit);
}

public class ReviewService(ICardRepository cardRepo) : IReviewService
{
    private static readonly int DueLimit = 50;
    private static readonly int NewLimit = 20;
    
    public async Task<ICollection<CardToReviewDto>> GetCardsToReviewAsync(
        Guid deckId,
        string userId,
        int dueLimit,
        int newLimit)
    {
        dueLimit = Math.Clamp(dueLimit, 0, DueLimit);
        newLimit = Math.Clamp(newLimit, 0, NewLimit);
        
        var nowUtc = DateTime.UtcNow;
        
        var dueCards = await cardRepo.GetDueCardsAsync(deckId, userId, dueLimit, nowUtc);

        if (newLimit == 0) return dueCards;
        
        var newCards = await cardRepo.GetNewCardsAsync(deckId, userId, newLimit);

        var result = new List<CardToReviewDto>();
        result.AddRange(dueCards);
        result.AddRange(newCards);
        
        return result;
    }

}