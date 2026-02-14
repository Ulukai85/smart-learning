using Microsoft.EntityFrameworkCore;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Services;

public interface IDeckService
{
    Task<DeckDto> CreateDeckAsync(CreateDeckDto dto, string userId);
    Task<ICollection<DeckDto>> GetAllDecksAsync();
}

public class DeckService(AppDbContext dbContext): IDeckService
{
    public async Task<DeckDto> CreateDeckAsync(CreateDeckDto dto,  string userId)
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
}