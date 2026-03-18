using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Repositories;

public interface IDeckRepository
{
    Task CreateDeckAsync(Deck deck);
    Task<ICollection<Deck>> GetAllDecksAsync();
    Task<ICollection<Deck>> GetDecksByUserIdAsync(string userId);
    Task<ICollection<DeckDto>> GetPublishedDecksAsync(string userId);
    Task<Deck?> GetDeckByIdAsync(Guid id);
    Task SaveChangesAsync();
    Task DeleteDeckAsync(Deck deck);
    Task<ICollection<DeckSummaryDto>> GetDeckSummariesByUserIdAsync(string userId);
    Task<ForkDeckDto> GetForkingDataByDeckId(Guid deckId);
    Task AddDeckWithCardsAsync(Deck deck, List<Card> cards);
    Task AddCardsAsync(List<Card> cards);
}