using Microsoft.EntityFrameworkCore;
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

    Task<ReviewResultDto> HandleReviewTransactionAsync(string userId, CreateReviewTransactionDto dto);
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

    public async Task<ReviewResultDto> HandleReviewTransactionAsync(string userId, CreateReviewTransactionDto dto)
    {
        var utcNow = DateTime.UtcNow;
        
        var card = await dbContext.Cards
            .Where(c => c.Id == dto.CardId)
            .Select(c => new
            {
                c.Id,
                c.DeckId,
                c.Deck.OwnerUserId,
            })
            .FirstOrDefaultAsync();
        
        if (card is null)
            throw new KeyNotFoundException("Card not found");
        
        if (card.OwnerUserId != userId)
            throw new UnauthorizedAccessException("Card not accessible");
        
        var deckId = card.DeckId;
        
        var progress = await dbContext.UserCardProgresses
            .FirstOrDefaultAsync(p => p.CardId == card.Id && p.UserId == userId);

        var wasNew = progress == null;

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
        
        var reinsertCard = UpdateSpacedRepetition(progress, dto.Grade, utcNow);

        var xpAmount = 5;
        
        var xpTransaction = new XpTransaction
        {
            UserId = userId,
            Amount = xpAmount,
            Reason = "CardReview",
            CreatedAt = utcNow
        };

        dbContext.XpTransactions.Add(xpTransaction);

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
        
        var dueCount = await cardRepo.CountDueCardsAsync(deckId, userId, utcNow);
        var newCount = await cardRepo.CountNewCardsAsync(deckId, userId);

        var result = new ReviewResultDto
        {
            ReviewedCardId = dto.CardId,
            ReinsertCard = reinsertCard,
            WasNew = wasNew,
            XpAmount = xpTransaction.Amount,
            XpReason = xpTransaction.Reason,
            UpdatedDueCount = dueCount,
            UpdatedNewCount = newCount,
            NextReviewAt = progress.NextReviewAt
        };
        
        return result;
    }

    private bool UpdateSpacedRepetition(UserCardProgress progress, int grade, DateTime utcNow)
    {
        progress.NextReviewAt = grade switch
        {
            0 => utcNow,
            1 => utcNow.AddDays(2),
            _ => utcNow.AddDays(4)
        };

        progress.LastReviewedAt = utcNow;
        progress.UpdatedAt = utcNow;

        var reinsertCard = grade == 0;

        return reinsertCard;
    }
}