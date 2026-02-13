using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartLearning.Models;

public class UserCardProgress
{
    public Guid Id { get; set; }
    
    [MaxLength(256)]
    public string UserId { get; set; } = null!;
    public AppUser User { get; set; } = null!;

    public Guid CardId { get; set; }
    public Card Card { get; set; } = null!;

    [MaxLength(256)]
    [Required]
    public string StrategyType { get; set; } = null!;
    
    [Column(TypeName = "json")]
    [MaxLength(2000)]
    [Required]
    public string StrategyDataJson { get; set; } = "{}";

    public DateTime? LastReviewedAt { get; set; }
    public DateTime NextReviewAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}