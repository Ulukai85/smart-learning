using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Models;

public class Deck
{
    public Guid Id { get; set; }
    
    public string OwnerUserId { get; set; } = null!;
    public AppUser OwnerUser { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;
    
    [MaxLength(2000)]
    public string? Description { get; set; }

    public bool IsPublished { get; set; }

    public Guid? SourceDeckId { get; set; }
    public Deck? SourceDeck { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Card> Cards { get; set; } = new List<Card>();
}