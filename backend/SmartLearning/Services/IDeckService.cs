using SmartLearning.DTOs;

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