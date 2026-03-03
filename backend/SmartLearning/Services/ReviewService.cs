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
    Task<StreakDto> GetStreakDataAsync(string userId);
}

public class ReviewService(
    ICardRepository cardRepo,
    IDeckRepository deckRepo,
    IReviewRepository reviewRepo,
    ITransactionRepository transactionRepo,
    IProgressRepository progressRepo,
    ISpacedRepetitionFactory spacedRepetitionFactory,
    ITimeProvider timeProvider
    ) : IReviewService
{
    private const int DueLimit = 50;
    private const int NewLimit = 20;
    private const string DefaultStrategyType = "Anki";
    private const int DefaultXpAmount = 5;

    public async Task<StreakDto> GetStreakDataAsync(string userId)
    {
        if (userId is null)
            throw new KeyNotFoundException("User not found");
        
        var dates = await reviewRepo.GetDistinctReviewDatesAsync(userId);
    }

    private (int Longest, int Current) CalculateStreaks(HashSet<DateOnly> dates)
    {
        if (dates.Count == 0)
            return (0, 0);
        
        var longestStreak = 0;
        
        foreach (var date in dates) 
        {
            if (!dates.Contains(date.AddDays(-1)))
            {
                int length = 1;
                var current = date;

                while (dates.Contains(current.AddDays(1)))
                {
                    current = current.AddDays(1);
                    length++;
                }

                longestStreak = Math.Max(longestStreak, length);
            }
        }

        int currentStreak = 0;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var startDate = dates.Contains(today)
            ? today
            : dates.Contains(today.AddDays(-1))
                ? today.AddDays(-1)
                : (DateOnly?)null;

        if (startDate.HasValue)
        {
            var current = startDate.Value;

            while (dates.Contains(current))
            {
                currentStreak++;
                current = current.AddDays(-1);
            }
        }

        return (longestStreak, currentStreak);
    } 
    
    
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
        
        var dueCount = await cardRepo.CountDueCardsAsync(deckId, userId, timeProvider.UtcNow);
        var newCount = await cardRepo.CountNewCardsAsync(deckId, userId);
        
        var dueCards = await cardRepo.GetDueCardsAsync(deckId, userId, dueLimit, timeProvider.UtcNow);
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
        var card = await LoadAndValidateCardAsync(userId, dto.CardId);
        var progress = await progressRepo.GetProgressAsync(userId, card.Id);
        var wasNew = progress == null;

        if (progress is null)
        {
            progress = BuildProgress(userId, dto, timeProvider.UtcNow);
            await progressRepo.AddProgressAsync(progress);
        }
        
        var strategy = spacedRepetitionFactory.GetStrategy(progress.StrategyType);
        UpdateProgressDates(progress, dto.Grade, timeProvider.UtcNow, strategy);
        
        var xpTransaction =  BuildXpTransaction(userId, timeProvider.UtcNow);
        await transactionRepo.AddXpTransactionAsync(xpTransaction);
        
        var reviewLog = BuildReviewLog(userId, dto, timeProvider.UtcNow);
        await reviewRepo.AddReviewLogAsync(reviewLog);

        await reviewRepo.SaveChangesAsync();
        
        var dueCount = await cardRepo.CountDueCardsAsync(card.DeckId, userId, timeProvider.UtcNow);
        var newCount = await cardRepo.CountNewCardsAsync(card.DeckId, userId);

        var result = new ReviewResultDto
        {
            ReviewedCardId = dto.CardId,
            ReinsertCard = strategy.ShouldReinsert(dto.Grade, progress.StrategyDataJson),
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
        var card = await cardRepo.GetCardByIdAsync(cardId);
        
        if (card is null)
            throw new KeyNotFoundException("Card not found");
        
        if (card.Deck.OwnerUserId != userId)
            throw new UnauthorizedAccessException("Card not accessible");
        
        return card;
    }

    private UserCardProgress BuildProgress(
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

    private XpTransaction BuildXpTransaction(string userId, DateTime utcNow)
    {
        return new XpTransaction
        {
            UserId = userId,
            Amount = DefaultXpAmount,
            Reason = "CardReview",
            CreatedAt = utcNow
        };
    }

    private ReviewLog BuildReviewLog(string userId, CreateReviewTransactionDto dto, DateTime utcNow)
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

    private void UpdateProgressDates(
        UserCardProgress progress,
        int grade,
        DateTime utcNow,
        ISpacedRepetitionStrategy strategy)
    {
        progress.NextReviewAt = strategy.CalculateNextReview(grade, utcNow, progress.StrategyDataJson);
        progress.LastReviewedAt = utcNow;
        progress.UpdatedAt = utcNow;
    }
}