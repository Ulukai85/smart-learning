using SmartLearning.DTOs;
using SmartLearning.Models;
using SmartLearning.Repositories;

namespace SmartLearning.Services;

public interface IReviewService
{
    Task<DeckToReviewDto> GetDeckToReviewAsync(
        Guid deckId,
        string userId,
        int dueLimit,
        int newLimit);

    Task<XpTransactionDto> HandleReviewTransactionAsync(string userId, CreateReviewTransactionDto dto);
}

public class ReviewService(
    ICardRepository cardRepo,
    IDeckRepository deckRepo,
    IReviewRepository reviewRepo,
    AppDbContext dbContext
    ) : IReviewService
{
    private const int DueLimit = 50;
    private const int NewLimit = 20;
    private const string DefaultStrategyType = "Anki";
    
    public async Task<DeckToReviewDto> GetDeckToReviewAsync(
        Guid deckId,
        string userId,
        int dueLimit,
        int newLimit)
    {
        dueLimit = Math.Clamp(dueLimit, 0, DueLimit);
        newLimit = Math.Clamp(newLimit, 0, NewLimit);
        
        var deck = await deckRepo.GetDeckByIdAsync(deckId);
        
        if (deck is null || deck.OwnerUserId != userId)
            throw new UnauthorizedAccessException("Deck not accessible");
        
        var nowUtc = DateTime.UtcNow;
        
        var dueCount = await cardRepo.CountDueCardsAsync(deckId, userId, nowUtc);
        var newCount = await cardRepo.CountNewCardsAsync(deckId, userId);
        
        var dueCards = await cardRepo.GetDueCardsAsync(deckId, userId, dueLimit, nowUtc);
        var newCards = await cardRepo.GetNewCardsAsync(deckId, userId, newLimit);

        var cards = new List<CardToReviewDto>();
        cards.AddRange(dueCards);
        cards.AddRange(newCards);
        
        return new DeckToReviewDto
        {
            Id = deckId,
            Name = deck.Name,
            DueCards = dueCount,
            NewCards = newCount,
            Cards = cards
        };
    }

    public async Task<XpTransactionDto> HandleReviewTransactionAsync(string userId, CreateReviewTransactionDto dto)
    {
        var utcNow = DateTime.UtcNow;
        
        var progress = await reviewRepo.GetUserCardProgress(userId, dto.CardId);

        if (progress is null)
        {
            progress = new UserCardProgress
            {
                UserId = userId,
                CardId = dto.CardId,
                StrategyType = dto.StrategyType ?? DefaultStrategyType,
                LastReviewedAt = utcNow,
                CreatedAt = utcNow,
            };

            dbContext.UserCardProgresses.Add(progress);
        }
        
        UpdateSpacedRepetition(progress, dto.Grade, utcNow);
        
        var transaction = new XpTransaction
        {
            UserId = userId,
            Amount = 5,
            Reason = "CardReview",
            CreatedAt = utcNow
        };

        dbContext.XpTransactions.Add(transaction);

        var log = new ReviewLog
        {
            UserId = userId,
            CardId = dto.CardId,
            StrategyType = dto.StrategyType ?? DefaultStrategyType,
            Grade = dto.Grade,
            ReviewedAt = utcNow
        };

        dbContext.ReviewLog.Add(log);

        await dbContext.SaveChangesAsync();
        
        return transaction.MapToDto();
    }

    private void UpdateSpacedRepetition(UserCardProgress progress, int grade, DateTime utcNow)
    {
        progress.NextReviewAt = grade switch
        {
            0 => utcNow.AddDays(4),
            1 => utcNow.AddDays(2),
            _ => utcNow.AddMinutes(5)
        };

        progress.LastReviewedAt = utcNow;
        progress.UpdatedAt = utcNow;
    }
}