using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Services;

public interface ICardService
{
    Task<Card> CreateCardAsync(UpsertCardDto dto);
    Task<ICollection<CardDto>> GetAllCardsAsync();
    Task<ICollection<CardDto>> GetCardsByUserIdAsync(string userId);
    Task UpdateCardAsync(Guid id, UpsertCardDto dto, string userId);
    Task DeleteCardAsync(Guid id, string userId);
}