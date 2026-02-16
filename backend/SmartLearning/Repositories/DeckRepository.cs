using Microsoft.EntityFrameworkCore;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface IDeckRepository
{
    Task CreateDeckAsync(Deck deck);
    Task<ICollection<Deck>> GetAllDecksAsync();
    Task<Deck?> GetDeckByIdAsync(Guid id);
    Task SaveChangesAsync();
    Task DeleteDeckAsync(Deck deck);
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
}