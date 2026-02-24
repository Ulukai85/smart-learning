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
    private const int DefaultXpAmount = 5;
    
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
        
        var utcNow = DateTime.UtcNow;
        
        var dueCount = await cardRepo.CountDueCardsAsync(deckId, userId, utcNow);
        var newCount = await cardRepo.CountNewCardsAsync(deckId, userId);
        
        var dueCards = await cardRepo.GetDueCardsAsync(deckId, userId, dueLimit, utcNow);
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
        
        var card = await LoadAndValidateCardAsync(userId, dto.CardId);
        
        var progress = await reviewRepo.GetUserCardProgress(userId, card.Id);
        var wasNew = progress == null;
        progress ??= CreateProgress(userId, dto, utcNow);
        dbContext.UserCardProgresses.Add(progress);
        
        var xpTransaction =  CreateXpTransaction(userId, utcNow);
        dbContext.XpTransactions.Add(xpTransaction);
        
        var reviewLog = CreateReviewLog(userId, dto, utcNow);
        dbContext.ReviewLog.Add(reviewLog);

        await dbContext.SaveChangesAsync();
        
        var dueCount = await cardRepo.CountDueCardsAsync(card.DeckId, userId, utcNow);
        var newCount = await cardRepo.CountNewCardsAsync(card.DeckId, userId);

        var result = new ReviewResultDto
        {
            ReviewedCardId = dto.CardId,
            ReinsertCard = UpdateSpacedRepetition(progress, dto.Grade, utcNow),
            WasNew = wasNew,
            XpAmount = xpTransaction.Amount,
            XpReason = xpTransaction.Reason,
            UpdatedDueCount = dueCount,
            UpdatedNewCount = newCount,
            NextReviewAt = progress.NextReviewAt
        };
        
        return result;
    }

    private async Task<Card> LoadAndValidateCardAsync(string userId, Guid cardId)
    {
        var card = await dbContext.Cards
            .Include(c => c.Deck)
            .FirstOrDefaultAsync(c => c.Id == cardId);
        
        if (card is null)
            throw new KeyNotFoundException("Card not found");
        
        if (card.Deck.OwnerUserId != userId)
            throw new UnauthorizedAccessException("Card not accessible");
        
        return card;
    }

    private UserCardProgress CreateProgress(
        string userId, 
        CreateReviewTransactionDto dto,
        DateTime utcNow)
    {
        return new UserCardProgress
        {
            UserId = userId,
            CardId = dto.CardId,
            StrategyType = dto.StrategyType ?? DefaultStrategyType,
            LastReviewedAt = utcNow,
            CreatedAt = utcNow,
        };
    }

    private XpTransaction CreateXpTransaction(string userId, DateTime utcNow)
    {
        return new XpTransaction
        {
            UserId = userId,
            Amount = DefaultXpAmount,
            Reason = "CardReview",
            CreatedAt = utcNow
        };
    }

    private ReviewLog CreateReviewLog(string userId, CreateReviewTransactionDto dto, DateTime utcNow)
    {
        return new ReviewLog
        {
            UserId = userId,
            CardId = dto.CardId,
            StrategyType = dto.StrategyType ?? DefaultStrategyType,
            Grade = dto.Grade,
            ReviewedAt = utcNow
        };
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