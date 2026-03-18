using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public partial interface ICardRepository
{
    Task<Card> CreateCardAsync(Card card);
    Task<ICollection<Card>> GetAllCardsAsync();
    Task<ICollection<Card>> GetCardsByUserIdAsync(string userId);
    Task<Card?> GetCardByIdAsync(Guid id);
    Task SaveChangesAsync();
    Task DeleteCardAsync(Card card);

    Task<int> CountDueCardsAsync(
        Guid deckId,
        string userId,
        DateTime nowUtc);

    Task<int> CountNewCardsAsync(
        Guid deckId,
        string userId);

    Task<ICollection<CardToReviewDto>> GetDueCardsAsync(
        Guid deckId,
        string userId,
        int limit,
        DateTime nowUtc);

    Task<ICollection<CardToReviewDto>> GetNewCardsAsync(
        Guid deckId,
        string userId,
        int limit
    );
}