using System.Text.Json;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using SmartLearning.DTOs;
using SmartLearning.Models;
using SmartLearning.Repositories;

namespace SmartLearning.Services;

public interface IAiService
{
    Task<string> GetResponseAsync(string prompt);
    Task<AICardResponse> GenerateCardsAsync(AiCreateCardsDto dto, string userId);
}

public class OpenAiService : IAiService
{
    private readonly IDeckRepository _deckRepo;
    private readonly ChatClient _client;
    private const string Model = "gpt-4.1-mini";
    private const int MaxCards = 20;

    public OpenAiService(IOptions<OpenAiSettings> openAiOptions, IDeckRepository deckRepo)
    {
        var openAiSettings = openAiOptions.Value;
        this._deckRepo = deckRepo;
        
        _client = new ChatClient(Model, openAiSettings.ApiKey);
    }

    public async Task<string> GetResponseAsync(string prompt)
    {
        var completion = await _client.CompleteChatAsync(prompt);
        
        return completion.Value.Content[0].Text;
    }
    
    public async Task<AICardResponse> GenerateCardsAsync(AiCreateCardsDto dto, string userId)
    {
        var count = Math.Clamp(dto.Count, 1, MaxCards);
        
        var options = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "list_of_cards",
                jsonSchema: BinaryData.FromBytes("""
                 {
                     "type": "object",
                     "properties": {
                         "Cards": {
                             "type": "array",
                             "items": {
                                 "type": "object",
                                 "properties": {
                                     "Front": { "type": "string" },
                                     "Back": { "type": "string" }
                                 },
                                 "required": ["Front", "Back"],
                                 "additionalProperties": false
                             }
                         }
                     },
                     "required": ["Cards"],
                     "additionalProperties": false
                 }
                 """u8.ToArray()),
                jsonSchemaIsStrict: true)
        };

        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage("""
                You generate high-quality learning flashcards adhering to the provided format.
                Each card should contain:
                - a clear question on the front
                - a concise answer on the back
                Answers should be 1-2 sentences.
                """),
            ChatMessage.CreateUserMessage(
                $"Create {count} flashcards about '{dto.Topic} with the following description in mind: {dto.Description}")
        };
            
        var completion = await _client.CompleteChatAsync(messages, options);
        var json = completion.Value.Content[0].Text;
        var result = JsonSerializer.Deserialize<AICardResponse>(json);

        if (result?.Cards is null || result.Cards.Count == 0)
        {
            throw new Exception("AI returned no cards");
        }

        await SaveGeneratedCards(userId, dto, result.Cards);

        return result;
    }
    
    private async Task SaveGeneratedCards(string userId, AiCreateCardsDto dto, List<AiCard> cards)
    {
        if (dto.DeckId.HasValue)
        {
            var deck = await _deckRepo.GetDeckByIdAsync(dto.DeckId.Value);
            
            if (deck?.OwnerUserId != userId)
                throw new UnauthorizedAccessException("Not accessible");
            
            if  (deck == null)
                throw new KeyNotFoundException("Deck not found");

            await _deckRepo.AddCardsAsync(MapAiCardsToCards(cards, deck.Id));
        }
        else
        {
            var newDeck = new Deck
            {
                Id = Guid.NewGuid(),
                Name = dto.Topic,
                Description = dto.Description,
                OwnerUserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _deckRepo.AddDeckWithCardsAsync(newDeck, MapAiCardsToCards(cards, newDeck.Id));
            
        }
        
        await _deckRepo.SaveChangesAsync();
    }
    
    private List<Card> MapAiCardsToCards(List<AiCard> aiCards, Guid deckId)
    {
        return aiCards.Select(c => new Card
        {
            Id = Guid.NewGuid(),
            DeckId = deckId,
            Front = c.Front,
            Back = c.Back,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();
    }
}