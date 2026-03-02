using SmartLearning.DTOs;
using SmartLearning.Models;
using SmartLearning.Repositories;

namespace SmartLearning.Services;

public interface IDeckService
{
    Task<DeckDto> CreateDeckAsync(UpsertDeckDto dto, string userId);
    Task<ICollection<DeckDto>> GetAllDecksAsync();
    Task<ICollection<DeckDto>> GetPublishedDecksAsync(string userId);
    Task<ICollection<DeckDto>> GetDecksByUserIdAsync(string userId);
    Task UpdateDeckAsync(Guid id, UpsertDeckDto dto, string userId);
    Task<ICollection<DeckSummaryDto>> GetDeckSummariesByUserIdAsync(string userId);
    Task SetIsPublishedAsync(bool isPublished, string userId, Guid deckId);
    Task DeleteDeckAsync(Guid id, string userId);
    Task<Guid> ForkDeckAsync(Guid deckId, string userId);
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
    
    public async Task<ICollection<DeckDto>> GetPublishedDecksAsync(string userId)
    {
        var decks = await deckRepo.GetPublishedDecksAsync(userId);
        
        return decks.Select(d => d.MapToDto()).ToList();
    }
    
    public async Task<ICollection<DeckDto>> GetDecksByUserIdAsync(string userId)
    {
        var decks = await deckRepo.GetDecksByUserIdAsync(userId);
        
        return decks.Select(d => d.MapToDto()).ToList();
    }
    
    public async Task UpdateDeckAsync(Guid id, UpsertDeckDto dto, string userId)
    {
        var deck = await deckRepo.GetDeckByIdAsync(id);
        
        if (deck?.OwnerUserId != userId)
            throw new UnauthorizedAccessException("Not accessible");
        
        if  (deck == null)
            throw new KeyNotFoundException("Deck not found");
        
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
    
    public async Task DeleteDeckAsync(Guid id, string userId)
    {
        var deck = await deckRepo.GetDeckByIdAsync(id);
        
        if (deck?.OwnerUserId != userId)
            throw new UnauthorizedAccessException("Not accessible");
        
        if  (deck == null)
            throw new KeyNotFoundException("Deck not found");
        
        await deckRepo.DeleteDeckAsync(deck);
    }

    public async Task<Guid> ForkDeckAsync(Guid deckId, string userId)
    {
        var forkData = await deckRepo.GetForkingDataByDeckId(deckId);
        
        if  (forkData == null)
            throw new KeyNotFoundException("Deck not found");

        var newDeck = new Deck
        {
            Id = Guid.NewGuid(),
            Name = forkData.Name + " (forked)",
            Description = forkData.Description,
            OwnerUserId = userId,
            SourceDeckId = deckId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var newCards = forkData.Cards
            .Select(c => new Card
            {
                Id = Guid.NewGuid(),
                DeckId = newDeck.Id,
                Front = c.Front,
                Back = c.Back,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            })
            .ToList();
        
        await deckRepo.AddDeckWithCardsAsync(newDeck, newCards);
        await deckRepo.SaveChangesAsync();
        
        return  newDeck.Id;
    }
}