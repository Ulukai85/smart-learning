using Microsoft.EntityFrameworkCore;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public class DeckRepository(AppDbContext dbContext) : IDeckRepository
{
    public async Task CreateDeckAsync(Deck deck)
    {
        await dbContext.Decks.AddAsync(deck);
        await SaveChangesAsync();
    }

    public async Task<ICollection<Deck>> GetAllDecksAsync()
    {
        return await dbContext.Decks
            .ToListAsync();
    }
    
    public async Task<ICollection<Deck>> GetDecksByUserIdAsync(string userId)
    {
        return await dbContext.Decks
            .Where(d => d.OwnerUserId == userId)
            .ToListAsync();
    }
    
    public async Task<ICollection<DeckDto>> GetPublishedDecksAsync(string userId)
    {
        return await dbContext.Decks
            .Where(d => 
                d.IsPublished && 
                d.OwnerUserId != userId &&
                d.SourceDeckId == null &&
                !dbContext.Decks.Any(b => 
                    b.SourceDeckId == d.Id &&
                    b.OwnerUserId == userId)
            )
            .Select(d => new DeckDto
            {
                Id = d.Id,
                OwnerUserId = d.OwnerUserId,
                Name = d.Name,
                Description = d.Description,
                TotalCards = d.Cards.Count()
            })
            .ToListAsync();
    }

    public async Task<Deck?> GetDeckByIdAsync(Guid id)
    {
        return await dbContext.Decks.FindAsync(id);
    }
    
    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteDeckAsync(Deck deck)
    {
        dbContext.Decks.Remove(deck);
        await SaveChangesAsync();
    }

    public async Task<ICollection<DeckSummaryDto>> GetDeckSummariesByUserIdAsync(string userId)
    {
        var now = DateTime.UtcNow;
        return await dbContext.Decks
            .Where(deck => deck.OwnerUserId == userId)
            .Select(deck => new DeckSummaryDto
            {
                Id = deck.Id,
                Name = deck.Name,
                IsPublished = deck.IsPublished,
                IsForked = deck.SourceDeckId != null,
                NewCards = deck.Cards
                    .Count(card => !dbContext.UserCardProgresses
                        .Any(progress => progress.CardId == card.Id
                                         && progress.UserId == userId)),
                DueCards = deck.Cards
                    .Count(card => dbContext.UserCardProgresses
                        .Any(progress => progress.UserId == userId
                                         && progress.CardId == card.Id
                                         && progress.NextReviewAt <= now))
            })
            .ToListAsync();
    }

    public async Task<ForkDeckDto> GetForkingDataByDeckId(Guid deckId)
    {
        return await dbContext.Decks
            .Where(d => d.Id == deckId)
            .Select(d => new ForkDeckDto
            {
                Name = d.Name,
                Description = d.Description,
                Cards = d.Cards.Select(c => new ForkCardDto
                {
                    Front = c.Front,
                    Back = c.Back
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddDeckWithCardsAsync(Deck deck, List<Card> cards)
    {
        await dbContext.Decks.AddAsync(deck);
        await dbContext.Cards.AddRangeAsync(cards);
    }

    public async Task AddCardsAsync(List<Card> cards)
    {
        await dbContext.Cards.AddRangeAsync(cards);
    }
}