using SmartLearning.DTOs;
using SmartLearning.Models;
using SmartLearning.Repositories;

namespace SmartLearning.Services;

public interface IDeckService
{
    Task<DeckDto> CreateDeckAsync(UpsertDeckDto dto, string userId);
    Task<ICollection<DeckDto>> GetAllDecksAsync();
    Task UpdateDeckAsync(Guid id, UpsertDeckDto dto);
    Task<ICollection<DeckSummaryDto>> GetDeckSummariesByUserIdAsync(string userId);
    Task SetIsPublishedAsync(bool isPublished, string userId, Guid deckId);

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
    
    public async Task<ICollection<DeckDto>> GetPublishedDecksAsync()
    {
        var decks = await deckRepo.GetPublishedDecksAsync();
        
        return decks.Select(d => d.MapToDto()).ToList();
    }
    
    public async Task<ICollection<DeckDto>> GetDecksByUserIdAsync(string userId)
    {
        var decks = await deckRepo.GetDecksByUserIdAsync(userId);
        
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

    public async Task<ICollection<DeckSummaryDto>> GetDeckSummariesByUserIdAsync(string userId)
    {
        var decks = await deckRepo.GetDeckSummariesByUserIdAsync(userId);
        return decks;
    }
    
    public async Task SetIsPublishedAsync(bool isPublished, string userId, Guid deckId) 
    {
        var deck = await deckRepo.GetDeckByIdAsync(deckId);

        if (deck?.OwnerUserId != userId)
            throw new UnauthorizedAccessException("Not accessible");
        
        if  (deck == null)
            throw new KeyNotFoundException("Deck not found");

        deck.IsPublished = isPublished;
        await deckRepo.SaveChangesAsync();
    }
}