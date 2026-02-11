using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SmartLearning.Models;

public class AppUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "varchar(256)")]
    public string Handle { get; set; } = null!;
}