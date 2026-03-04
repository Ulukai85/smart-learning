namespace SmartLearning.DTOs;

public class CreateReviewTransactionDto
{
    public Guid CardId { get; set; }
    public int Grade { get; set; }
    public string? StrategyType { get; set; }
    public string? StrategyData { get; set; }
}

public class XpTransactionDto
{
    public Guid Id { get; set; }
    public int Amount { get; set; }
    public string Reason { get; set; }
}

public class ReviewResultDto
{
    public Guid ReviewedCardId { get; set; }
    public bool ReinsertCard { get; set; }
    public bool WasNew { get; set; }
    public int XpAmount { get; set; }
    public string? XpReason { get; set; }
    public int UpdatedDueCount  { get; set; }
    public int UpdatedNewCount  { get; set; }
    public DateTime? NextReviewAt { get; set; }
}