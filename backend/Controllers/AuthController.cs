using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration config) : ControllerBase
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

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(
        UserManager<AppUser> userManager, [FromBody] UserLoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user != null && await userManager.CheckPasswordAsync(user, dto.Password))
        {
            var signInKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config.GetSection("AppSettings:JWTSecret").Value!));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim("UserId", user.Id)
                ]),
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return Ok(new {token});

        }
        else
        {
            return BadRequest(new {message = "Username or password is incorrect"});
        }
    }
}
