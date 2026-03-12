using Microsoft.AspNetCore.Mvc;
using SmartLearning.DTOs;
using SmartLearning.Services;

namespace SmartLearning.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController(IAiService aiService) : ControllerBase
{
    [HttpPost("test")]
    public async Task<IActionResult> SignUp([FromBody] AiRequestDto dto)
    {
        var response = await aiService.GetResponseAsync(dto.Prompt);
        return Ok( new { response });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCards([FromBody] AiCreateCardsDto dto)
    {
        try
        {
            var response = await aiService.GenerateCardsAsync(dto);
            return Ok(response);
        } 
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    } 
}