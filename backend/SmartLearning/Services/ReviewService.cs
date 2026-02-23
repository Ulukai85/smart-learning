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
    ITransactionRepository transactionRepo,
    IReviewRepository reviewRepo
    ) : IReviewService
{
    private const int DueLimit = 50;
    private const int NewLimit = 20;
    
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
                StrategyType = dto.StrategyType,
                LastReviewedAt = utcNow,
                NextReviewAt = utcNow.AddDays(1),
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };

            progress = await reviewRepo.CreateUserCardProgress(progress);
        } else
        {
            progress.LastReviewedAt = utcNow;
            progress.NextReviewAt = utcNow.AddDays(1);
            progress.UpdatedAt = utcNow;

            await reviewRepo.SaveChangesAsync();
        }
        
        var transaction = new XpTransaction
        {
            UserId = userId,
            Amount = 5,
            Reason = "CardReview",
            CreatedAt = utcNow
        };

        var savedTransaction = await transactionRepo.SaveXpTransaction(transaction);

        var log = new ReviewLog
        {
            UserId = userId,
            CardId = dto.CardId,
            StrategyType = dto.StrategyType,
            Grade = dto.Grade,
            ReviewedAt = utcNow
        };

        var reviewLog = await reviewRepo.SaveReviewLog(log);

        return savedTransaction.MapToDto();
    }
    

}