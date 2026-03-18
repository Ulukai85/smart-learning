using SmartLearning.DTOs;

namespace SmartLearning.Services;

public interface IAiService
{
    Task<string> GetResponseAsync(string prompt);
    Task<AICardResponseDto> GenerateCardsAsync(AiCreateCardDto dtos, string userId);
}