namespace SmartLearning.DTOs;

public class CardDto
{
    public Guid Id { get; set; }
    public Guid DeckId { get; set; }
    public required string DeckName { get; set; }
    public string Front { get; set; } = null!;
    public string Back { get; set; } = null!;
}

public class UpsertCardDto
{
    public Guid DeckId { get; set; }
    public string Front { get; set; } = null!;
    public string Back { get; set; } = null!;
}

public class CardToReviewDto
{
    public Guid Id { get; set; }
    public required string Front { get; set; }
    public required string Back { get; set; }
    public DateTime? NextReviewAt { get; set; }
    public bool IsNew { get; set; }
}

public class ForkCardDto
{
    public required string Front { get; set; }
    public required string Back { get; set; }
}