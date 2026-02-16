using Microsoft.EntityFrameworkCore;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface ICardRepository
{
    Task CreateCardAsync(Card card);
    Task<ICollection<Card>> GetAllCardsAsync();
    Task<Card?> GetCardByIdAsync(Guid id);
    Task SaveChangesAsync();
    Task DeleteCardAsync(Card card);
}

public class CardRepository(AppDbContext dbContext) : ICardRepository 
{
    public async Task CreateCardAsync(Card card)
    {
        await dbContext.Cards.AddAsync(card);
        await SaveChangesAsync();
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
}