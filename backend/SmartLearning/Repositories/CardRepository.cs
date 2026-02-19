using Microsoft.EntityFrameworkCore;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface ICardRepository
{
    Task<Card> CreateCardAsync(Card card);
    Task<ICollection<Card>> GetAllCardsAsync();
    Task<Card?> GetCardByIdAsync(Guid id);
    Task SaveChangesAsync();
    Task DeleteCardAsync(Card card);
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

public class CardRepository(AppDbContext dbContext) : ICardRepository 
{
    public async Task<Card> CreateCardAsync(Card card)
    {
        await dbContext.Cards.AddAsync(card);
        await SaveChangesAsync();
        
        return card;
    }

    public async Task<ICollection<Card>> GetAllCardsAsync()
    {
        return await dbContext.Cards
            .Include(c => c.Deck)
            .ToListAsync();
    }

    public async Task<Card?> GetCardByIdAsync(Guid id)
    {
        return await dbContext.Cards.FindAsync(id);
    }
    
    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteCardAsync(Card card)
    {
        dbContext.Cards.Remove(card);
        await SaveChangesAsync();
    }

    public async Task<ICollection<CardToReviewDto>> GetDueCardsAsync(
        Guid deckId,
        string userId,
        int limit,
        DateTime nowUtc)
    {
        return await dbContext.UserCardProgresses
            .Where(p => p.UserId == userId 
                        && p.Card.DeckId == deckId
                        && p.NextReviewAt <= nowUtc)
            .OrderBy(p => p.NextReviewAt)
            .Select(p => new CardToReviewDto
            {
                Id = p.Card.Id,
                Front = p.Card.Front,
                Back = p.Card.Back,
                NextReviewAt = p.NextReviewAt,
                IsNew = false
            })
            .Take(limit)
            .ToListAsync();
    }

    public async Task<ICollection<CardToReviewDto>> GetNewCardsAsync(
        Guid deckId,
        string userId,
        int limit
    )
    {
        return await dbContext.Cards
            .Where(c => c.DeckId == deckId)
            .Where(c => !dbContext.UserCardProgresses
                .Any(p => p.UserId == userId && p.CardId == c.Id))
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CardToReviewDto
            {
                Id = c.Id,
                Front = c.Front,
                Back = c.Back,
                NextReviewAt = null,
                IsNew = true
            })
            .Take(limit)
            .ToListAsync();
    }
}