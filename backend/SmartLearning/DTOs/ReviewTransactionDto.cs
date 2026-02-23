namespace SmartLearning.DTOs;

public class CreateReviewTransactionDto
{
    public Guid CardId { get; set; }
    public int Grade { get; set; }
    public string? StrategyType { get; set; }
}

public class XpTransactionDto
{
    public Guid Id { get; set; }
    public int Amount { get; set; }
    public string Reason { get; set; }
}

