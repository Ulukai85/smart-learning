using Microsoft.Extensions.Options;
using OpenAI.Chat;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Services;

public interface IAiService
{
    Task<string> GetResponseAsync(string prompt);
}

public class OpenAiService : IAiService
{
    private readonly ChatClient client;
    private const string model = "gpt-4.1-mini";

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
    
    public async Task<string> GenerateCardsAsync(AiCreateCardsDto dto)
    {
        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "list_of_cards",
                jsonSchema: BinaryData.FromBytes("""
                 {
                     "type": "object",
                     "properties": {
                         "steps": {
                             "type": "array",
                             "items": {
                                 "type": "object",
                                 "properties": {
                                     "explanation": { "type": "string" },
                                     "output": { "type": "string" }
                                 },
                                 "required": ["explanation", "output"],
                                 "additionalProperties": false
                             }
                         },
                         "final_answer": { "type": "string" }
                     },
                     "required": ["steps", "final_answer"],
                     "additionalProperties": false
                 }
                 """u8.ToArray()),
                jsonSchemaIsStrict: true)
        };

            
        
        var prompt = $"""
            Create {dto.Count} flashcards for studying the topic: "{dto.Topic}".
            Return ONLY valid JSON in the provided format:

            Guidelines:
            - Clean and concise
            - Suitable for learning
            - No explanations outside JSON
            - Just 1 or 2 sentences per answer
            """;
            
        var completion = await client.CompleteChatAsync(prompt);
        
        return completion.Value.Content[0].Text;
    }
}