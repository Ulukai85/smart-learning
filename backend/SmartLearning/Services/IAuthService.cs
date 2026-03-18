using Microsoft.AspNetCore.Identity;
using SmartLearning.DTOs;

namespace SmartLearning.Services;

public interface IAuthService
{
    Task<IdentityResult> SignUpAsync(UserRegistrationDto dto);
    Task<string> SignInAsync(UserLoginDto dto);
}