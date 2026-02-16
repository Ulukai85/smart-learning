using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.DTOs;
using SmartLearning.Services;

namespace SmartLearning.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] UserRegistrationDto dto)
    {
        var result = await authService.SignUpAsync(dto);
        if (result.Succeeded) return Ok(result);

        return BadRequest(result);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] UserLoginDto dto)
    {
        try
        {
            var token = await authService.SignInAsync(dto);
            return Ok(new { token });
        }
        catch (AuthenticationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}