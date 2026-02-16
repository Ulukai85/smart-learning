using Microsoft.EntityFrameworkCore;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Services;

public interface ICardService
{
    Task CreateCardAsync(UpsertCardDto dto);
    Task<ICollection<CardDto>> GetAllCardsAsync();
    Task UpdateCardAsync(Guid id, UpsertCardDto dto);
    Task DeleteCardAsync(Guid id);
}

public class CardService(AppDbContext dbContext): ICardService
{
    public async Task CreateCardAsync(UpsertCardDto dto)
    {
        var card = new Card
        {
            DeckId = dto.DeckId,
            Front =  dto.Front,
            Back =  dto.Back,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await dbContext.Cards.AddAsync(card);
        await dbContext.SaveChangesAsync();
    }

    public async Task<ICollection<CardDto>> GetAllCardsAsync()
    {
        var cards = await dbContext.Cards
            .Include(d => d.Deck)
            .ToListAsync();
        
        return cards.Select(d => d.MapToDto()).ToList();
    }

    public async Task UpdateCardAsync(Guid id, UpsertCardDto dto)
    {
        var card = await dbContext.Cards.FindAsync(id) ?? throw new KeyNotFoundException("Card not found");
        
        card.DeckId = dto.DeckId;
        card.Front = dto.Front;
        card.Back = dto.Back;
        
        card.UpdatedAt = DateTime.UtcNow;
        
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteCardAsync(Guid id)
    {
        var card = await dbContext.Cards.FindAsync(id) ?? throw new KeyNotFoundException("Card not found");
        dbContext.Cards.Remove(card);
        await dbContext.SaveChangesAsync();
    }
}