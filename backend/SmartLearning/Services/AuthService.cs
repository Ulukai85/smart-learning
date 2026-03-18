using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartLearning.DTOs;
using SmartLearning.Models;

namespace SmartLearning.Services;

public class AuthService(UserManager<AppUser> userManager, IOptions<JwtSettings> jwtOptions)
    : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<IdentityResult> SignUpAsync(UserRegistrationDto dto)
    {
        var user = new AppUser
        {
            Email = dto.Email,
            UserName = dto.Email,
            Handle = dto.Handle
        };
        return await userManager.CreateAsync(user, dto.Password);
    }

    public async Task<string> SignInAsync(UserLoginDto dto)
    { 
        var key = _jwtSettings.Key;
        var expiresMinutes = _jwtSettings.ExpiresMinutes;
        
        var user = await userManager.FindByEmailAsync(dto.Email);

        if (user == null || !await userManager.CheckPasswordAsync(user, dto.Password))
            throw new AuthenticationException("Invalid username or password");

        var signInKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Handle)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(expiresMinutes),
            SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }
}