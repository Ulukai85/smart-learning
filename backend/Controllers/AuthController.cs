using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(
        UserManager<AppUser> userManager, [FromBody] UserRegistrationDto dto)
    {
        var user = new AppUser()
        {
            Email = dto.Email,
            Handle = dto.Handle,
        };
        var result = await userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}
