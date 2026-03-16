namespace SmartLearning.DTOs;

public class AiRequestDto
{
    public string Prompt { get; set; }
}

public class AiCreateCardsDto 
{
    public int Count { get; set; }
    public string Topic { get; set; }
    public Guid? DeckId { get; set; }
    public string Description { get; set; }
    public string? SourceText { get; set; }
}

public class AICardResponse
{
    public List<AiCard> Cards { get; set; } = [];
}

public class AiCard
{
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
}