namespace SmartLearning.DTOs;

public class DeckDto
{
    public required Guid Id {get; set;}
    public required string OwnerUserId {get; set;}
    public required string Name {get; set;}
    public string Description { get; set; } = string.Empty;
    public int TotalCards { get; set; }
}

public class UpsertDeckDto
{
    public required string Name {get; set;}
    public string Description {get; set;} = string.Empty;
}

public class DeckSummaryDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsPublished { get; set; }
    public int NewCards { get; set; }
    public int DueCards { get; set; }
}

public class DeckToReviewDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int NewCards { get; set; }
    public int DueCards { get; set; }
    public List<CardToReviewDto> Cards { get; set; } = [];
}
