using Microsoft.EntityFrameworkCore;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Services;

public interface ICardService
{
    Task<CardDto> CreateCardAsync(CreateCardDto dto);
    Task<ICollection<CardDto>> GetAllCardsAsync();
}

public class CardService(AppDbContext dbContext): ICardService
{
    public async Task<CardDto> CreateCardAsync(CreateCardDto dto)
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
        
        return card.MapToDto();
    }

    public async Task<ICollection<CardDto>> GetAllCardsAsync()
    {
        var cards = await dbContext.Cards.ToListAsync();
        
        return cards.Select(d => d.MapToDto()).ToList();
    }
}