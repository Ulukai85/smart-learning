using Microsoft.EntityFrameworkCore;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface IDeckRepository
{
    Task CreateDeckAsync(Deck deck);
    Task<ICollection<Deck>> GetAllDecksAsync();
    Task<Deck?> GetDeckByIdAsync(Guid id);
    Task SaveChangesAsync();
    Task DeleteDeckAsync(Deck deck);
    Task<ICollection<DeckSummaryDto>> GetDeckSummariesByUserIdAsync(string userId);
}

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
            .Select(deck => new DeckSummaryDto
            {
                Id = deck.Id,
                Name = deck.Name,
                TotalCards = deck.Cards.Count,
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
}