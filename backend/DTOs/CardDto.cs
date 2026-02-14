namespace SmartLearning.DTOs;

public class CardDto
{
    public Guid Id { get; set; }
    public Guid DeckId { get; set; }
    public string Front { get; set; } = null!;
    public string Back { get; set; } = null!;
}

public class CreateCardDto
{
    public Guid DeckId { get; set; }
    public string Front { get; set; } = null!;
    public string Back { get; set; } = null!;
}