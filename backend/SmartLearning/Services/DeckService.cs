using SmartLearning.DTOs;
using SmartLearning.Models;
using SmartLearning.Repositories;

namespace SmartLearning.Services;

public interface IDeckService
{
    Task<DeckDto> CreateDeckAsync(UpsertDeckDto dto, string userId);
    Task<ICollection<DeckDto>> GetAllDecksAsync();
    Task UpdateDeckAsync(Guid id, UpsertDeckDto dto);
}

public class DeckService(IDeckRepository deckRepo): IDeckService
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
        
        await deckRepo.CreateDeckAsync(deck);
        
        return deck.MapToDto();
    }

    public async Task<ICollection<DeckDto>> GetAllDecksAsync()
    {
        var decks = await deckRepo.GetAllDecksAsync();
        
        return decks.Select(d => d.MapToDto()).ToList();
    }
    
    public async Task UpdateDeckAsync(Guid id, UpsertDeckDto dto)
    {
        var deck = await deckRepo.GetDeckByIdAsync(id) ?? throw new KeyNotFoundException("Deck not found");
        
        deck.Name = dto.Name;
        deck.Description = dto.Description;
        
        deck.UpdatedAt = DateTime.UtcNow;

        await deckRepo.SaveChangesAsync();
    }
}