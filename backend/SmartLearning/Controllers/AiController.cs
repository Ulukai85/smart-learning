using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.DTOs;
using SmartLearning.Services;

namespace SmartLearning.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AiController(IAiService aiService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateCards([FromBody] AiCreateCardDto dtos)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await aiService.GenerateCardsAsync(dtos, userId!);
            return Ok(response);
        } 
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    } 
}