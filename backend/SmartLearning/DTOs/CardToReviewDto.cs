namespace SmartLearning.DTOs;

public class CardToReviewDto
{
    public Guid Id { get; set; }
    public string Front { get; set; }
    public string Back { get; set; }
    public DateTime? NextReviewAt { get; set; }
    public bool IsNew { get; set; }
}