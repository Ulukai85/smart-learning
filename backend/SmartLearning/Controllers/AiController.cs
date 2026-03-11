using Microsoft.AspNetCore.Mvc;
using SmartLearning.DTOs;
using SmartLearning.Services;

namespace SmartLearning.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController(IAiService aiService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SignUp([FromBody] AiRequestDto dto)
    {
        var response = await aiService.GetResponseAsync(dto.prompt);
        return Ok( new { response });
    }
}