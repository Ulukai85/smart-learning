using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Models;

public class ReviewLog
{
    public Guid Id { get; set; }
    
    [MaxLength(256)]
    public string UserId { get; set; } = null!;
    public AppUser User { get; set; } = null!;
    
    public Guid CardId { get; set; }
    public Card Card { get; set; } = null!;

    [MaxLength(255)]
    [Required]
    public string StrategyType { get; set; } = null!;
    
    public DateTime ReviewedAt { get; set; }
    public int Grade { get; set; }
}