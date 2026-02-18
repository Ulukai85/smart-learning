using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Models;

public class XpTransaction
{
    public Guid Id { get; set; }
    
    [MaxLength(256)]
    public string UserId { get; set; }
    public AppUser User { get; set; } = null!;
    
    public int Amount { get; set; }
    
    [MaxLength(255)]
    public string Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}