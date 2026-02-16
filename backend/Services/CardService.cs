using SmartLearning.DTOs;
using SmartLearning.Models;
using SmartLearning.Repositories;

namespace SmartLearning.Services;

public interface ICardService
{
    Task CreateCardAsync(UpsertCardDto dto);
    Task<ICollection<CardDto>> GetAllCardsAsync();
    Task UpdateCardAsync(Guid id, UpsertCardDto dto);
    Task DeleteCardAsync(Guid id);
}

public class CardService(ICardRepository cardRepo): ICardService
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
        
        await cardRepo.CreateCardAsync(card);
    }

    public async Task<ICollection<CardDto>> GetAllCardsAsync()
    {
        var cards = await cardRepo.GetAllCardsAsync();
        
        return cards.Select(d => d.MapToDto()).ToList();
    }

    public async Task UpdateCardAsync(Guid id, UpsertCardDto dto)
    {
        var card = await cardRepo.GetCardByIdAsync(id) ?? throw new KeyNotFoundException("Card not found");
        
        card.DeckId = dto.DeckId;
        card.Front = dto.Front;
        card.Back = dto.Back;
        
        card.UpdatedAt = DateTime.UtcNow;

        await cardRepo.SaveChangesAsync();
    }

    public async Task DeleteCardAsync(Guid id)
    {
        var card = await cardRepo.GetCardByIdAsync(id) ?? throw new KeyNotFoundException("Card not found");
        
        await cardRepo.DeleteCardAsync(card);
    }
}