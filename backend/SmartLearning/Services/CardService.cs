using SmartLearning.DTOs;
using SmartLearning.Models;
using SmartLearning.Repositories;

namespace SmartLearning.Services;

public class CardService(ICardRepository cardRepo): ICardService
{
    public async Task<Card> CreateCardAsync(UpsertCardDto dto)
    {
        var card = new Card
        {
            DeckId = dto.DeckId,
            Front =  dto.Front,
            Back =  dto.Back,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        return await cardRepo.CreateCardAsync(card);
    }

    public async Task<ICollection<CardDto>> GetAllCardsAsync()
    {
        var cards = await cardRepo.GetAllCardsAsync();
        
        return cards.Select(d => d.MapToDto()).ToList();
    }

    public async Task<ICollection<CardDto>> GetCardsByUserIdAsync(string userId)
    {
        var cards = await cardRepo.GetCardsByUserIdAsync(userId);
        
        return cards.Select(d => d.MapToDto()).ToList();
    }

    public async Task UpdateCardAsync(Guid id, UpsertCardDto dto, string userId)
    {
        var card = await cardRepo.GetCardByIdAsync(id) ?? throw new KeyNotFoundException("Card not found");
        
        if (card?.Deck.OwnerUserId != userId)
            throw new UnauthorizedAccessException("Not accessible");
        
        if (card == null)
            throw new KeyNotFoundException("Card not found");
        
        card.DeckId = dto.DeckId;
        card.Front = dto.Front;
        card.Back = dto.Back;
        
        card.UpdatedAt = DateTime.UtcNow;

        await cardRepo.SaveChangesAsync();
    }

    public async Task DeleteCardAsync(Guid id, string userId)
    {
        var card = await cardRepo.GetCardByIdAsync(id);

        if (card?.Deck.OwnerUserId != userId)
            throw new UnauthorizedAccessException("Not accessible");
        
        if (card == null)
            throw new KeyNotFoundException("Card not found");
        
        await cardRepo.DeleteCardAsync(card);
    }
}