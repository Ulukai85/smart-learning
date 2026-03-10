using System.ComponentModel.DataAnnotations;
using SmartLearning.DTOs;

namespace SmartLearning.Models;

public class XpTransaction
{
    public Guid Id { get; set; }
    
    [MaxLength(256)]
    public required string UserId { get; set; }
    public AppUser User { get; set; } = null!;
    
    public int Amount { get; set; }
    
    [MaxLength(255)]
    public string Reason { get; set; }
    public DateTime CreatedAt { get; set; }


    public XpTransactionDto MapToDto()
    {
        return new XpTransactionDto
        {
            Amount = Amount,
            Reason = Reason
        };
    } 
}