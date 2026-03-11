using Microsoft.Extensions.Options;
using OpenAI.Chat;
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
}