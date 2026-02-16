using Microsoft.EntityFrameworkCore;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Services;

public interface IDeckService
{
    Task<DeckDto> CreateDeckAsync(UpsertDeckDto dto, string userId);
    Task<ICollection<DeckDto>> GetAllDecksAsync();
    Task UpdateDeckAsync(Guid id, UpsertDeckDto dto);
}

public class DeckService(AppDbContext dbContext): IDeckService
{
    public async Task<DeckDto> CreateDeckAsync(UpsertDeckDto dto,  string userId)
    {
        var deck = new Deck
        {
            Name = dto.Name,
            Description = dto.Description,
            OwnerUserId = userId,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await dbContext.Decks.AddAsync(deck);
        await dbContext.SaveChangesAsync();
        
        return deck.MapToDto();
    }

    public async Task<ICollection<DeckDto>> GetAllDecksAsync()
    {
        var decks = await dbContext.Decks.ToListAsync();
        
        return decks.Select(d => d.MapToDto()).ToList();
    }
    
    public async Task UpdateDeckAsync(Guid id, UpsertDeckDto dto)
    {
        var deck = await dbContext.Decks.FindAsync(id) ?? throw new KeyNotFoundException("Deck not found");
        
        deck.Name = dto.Name;
        deck.Description = dto.Description;
        
        deck.UpdatedAt = DateTime.UtcNow;
        
        await dbContext.SaveChangesAsync();
    }
}