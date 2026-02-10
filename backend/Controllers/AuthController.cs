using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Models;

namespace SmartLearning.Controllers;

public class UserRegistrationModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(
        UserManager<AppUser> userManager, [FromBody] UserRegistrationModel model)
    {
        var user = new AppUser()
        {
            Email = model.Email,
            UserName = model.UserName,

        };
        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}
