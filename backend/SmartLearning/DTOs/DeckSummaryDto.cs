namespace SmartLearning.DTOs;

public class DeckSummaryDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int TotalCards { get; set; }
    public int NewCards { get; set; }
    public int DueCards { get; set; }
}