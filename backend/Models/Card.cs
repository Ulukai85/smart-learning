using System.ComponentModel.DataAnnotations;
using SmartLearning.DTOs;

namespace SmartLearning.Models;

public class Card
{
    public Guid Id { get; set; }

    public Guid DeckId { get; set; }
    public Deck Deck { get; set; } = null!;
    
    [Required]
    [MaxLength(2000)]
    public string Front { get; set; } = null!;
    
    [Required]
    [MaxLength(2000)]
    public string Back { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<UserCardProgress> UserCardProgresses { get; set; } 
        = new List<UserCardProgress>();

    public CardDto MapToDto()
    {
        return new CardDto
        {
            Id = Id,
            DeckId = DeckId,
            Front = Front,
            Back = Back,
        };
    }
}