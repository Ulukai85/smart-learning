using Microsoft.EntityFrameworkCore;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

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
    
    public async Task<ICollection<Card>> GetCardsByUserIdAsync(string userId)
    {
        return await dbContext.Cards
            .Include(c => c.Deck)
            .Where(c => c.Deck.OwnerUserId == userId)
            .ToListAsync();
    }

    public async Task<Card?> GetCardByIdAsync(Guid id)
    {
        return await dbContext.Cards
            .Include(c => c.Deck)
            .FirstOrDefaultAsync(c => c.Id == id);
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

    public async Task<int> CountDueCardsAsync(
        Guid deckId,
        string userId,
        DateTime nowUtc)
    {
        return await DueQuery(deckId, userId, nowUtc).CountAsync();
    }
    
    public async Task<ICollection<CardToReviewDto>> GetDueCardsAsync(
        Guid deckId,
        string userId,
        int limit,
        DateTime nowUtc)
    {
        return await DueQuery(deckId, userId, nowUtc)
            .OrderBy(p => p.NextReviewAt)
            .Take(limit)
            .Select(p => new CardToReviewDto
            {
                Id = p.Card.Id,
                Front = p.Card.Front,
                Back = p.Card.Back,
                NextReviewAt = p.NextReviewAt,
                IsNew = false
            })
           
            .ToListAsync();
    }
    
    public async Task<int> CountNewCardsAsync(
        Guid deckId,
        string userId)
    {
        return await dbContext.Cards
            .Where(c =>
                c.DeckId == deckId &&
                !dbContext.UserCardProgresses
                    .Any(p =>
                        p.UserId == userId && 
                        p.CardId == c.Id))
            .CountAsync();
    }
    
    public async Task<ICollection<CardToReviewDto>> GetNewCardsAsync(
        Guid deckId,
        string userId,
        int limit
    )
    {
        return await dbContext.Cards
            .Where(c => c.DeckId == deckId &&
                        !dbContext.UserCardProgresses
                            .Any(p => 
                                p.UserId == userId && 
                                p.CardId == c.Id))
            .OrderBy(c => c.CreatedAt)
            .Take(limit)
            .Select(c => new CardToReviewDto
            {
                Id = c.Id,
                Front = c.Front,
                Back = c.Back,
                NextReviewAt = null,
                IsNew = true
            })
            .ToListAsync();
    }
    
    private IQueryable<UserCardProgress> DueQuery(
        Guid deckId,
        string userId,
        DateTime nowUtc)
    {
        return dbContext.UserCardProgresses
            .Where(p =>
                p.UserId == userId &&
                p.Card.DeckId == deckId &&
                p.NextReviewAt <= nowUtc);
    }
}