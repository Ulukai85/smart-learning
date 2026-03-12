namespace SmartLearning.DTOs;

public class AiRequestDto
{
    public string Prompt { get; set; }
}

public class AiCreateCardsDto 
{
    public int Count { get; set; }
    public string Topic { get; set; }
}