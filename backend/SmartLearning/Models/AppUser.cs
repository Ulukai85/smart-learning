using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SmartLearning.Models;

public class AppUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "varchar(255)")]
    public string Handle { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }

    public ICollection<UserCardProgress> UserCardProgresses { get; set; }
        = new List<UserCardProgress>();
}