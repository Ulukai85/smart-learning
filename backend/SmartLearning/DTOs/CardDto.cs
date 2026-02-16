namespace SmartLearning.DTOs;

public class CardDto
{
    public Guid Id { get; set; }
    public Guid DeckId { get; set; }
    public string DeckName { get; set; }
    public string Front { get; set; } = null!;
    public string Back { get; set; } = null!;
}

public class UpsertCardDto
{
    public Guid DeckId { get; set; }
    public string Front { get; set; } = null!;
    public string Back { get; set; } = null!;
}