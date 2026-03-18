namespace SmartLearning.DTOs;

public class AiCreateCardDto 
{
    public int Count { get; set; }
    public string Topic { get; set; }
    public Guid? DeckId { get; set; }
    public string Description { get; set; }
    public string? SourceText { get; set; }
}

public class AICardResponseDto
{
    public List<AiCardDto> Cards { get; set; } = [];
}

public class AiCardDto
{
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
}