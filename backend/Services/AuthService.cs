using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Services;

public interface IAuthService
{
    Task<IdentityResult> SignUpAsync(UserRegistrationDto dto);
    Task<string> SignInAsync(UserLoginDto dto);
}

public class AuthService(UserManager<AppUser> userManager, IConfiguration config) : IAuthService
{
    public async Task<IdentityResult> SignUpAsync(UserRegistrationDto dto)
    {
        var user = new AppUser
        {
            Email = dto.Email,
            Handle = dto.Handle
        };
        return await userManager.CreateAsync(user, dto.Password);
    }

    public async Task<string> SignInAsync(UserLoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);

        if (user == null || !await userManager.CheckPasswordAsync(user, dto.Password))
            throw new AuthenticationException("Invalid username or password");

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
        return tokenHandler.WriteToken(securityToken);
    }
}