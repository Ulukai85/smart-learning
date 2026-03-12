using System.Text.Json;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Services;

public class AICardResponse
{
    public List<AiCard> Cards { get; set; } = [];
}

public class AiCard
{
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
}

public interface IAiService
{
    Task<string> GetResponseAsync(string prompt);
    Task<AICardResponse> GenerateCardsAsync(AiCreateCardsDto dto);
}

public class OpenAiService : IAiService
{
    private readonly ChatClient client;
    private const string model = "gpt-4.1-mini";
    private const int maxCards = 20;

    public OpenAiService(IOptions<OpenAiSettings> openAiOptions)
    {
        var openAiSettings = openAiOptions.Value;
        
        client = new ChatClient(model, openAiSettings.ApiKey);
    }

    public async Task<string> GetResponseAsync(string prompt)
    {
        var completion = await client.CompleteChatAsync(prompt);
        
        return completion.Value.Content[0].Text;
    }
    
    public async Task<AICardResponse> GenerateCardsAsync(AiCreateCardsDto dto)
    {
        var count = Math.Clamp(dto.Count, 1, maxCards);
        
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
                $"Create {count} flashcards about: {dto.Topic}")
        };
            
        var completion = await client.CompleteChatAsync(messages, options);
        var json = completion.Value.Content[0].Text;
        var result = JsonSerializer.Deserialize<AICardResponse>(json);

        if (result?.Cards is null || result.Cards.Count == 0)
        {
            throw new Exception("AI returned no cards");
        }

        return result;
    }
}