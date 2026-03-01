using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.DTOs;
using SmartLearning.Services;

namespace SmartLearning.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CardsController(ICardService cardService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCard([FromBody] UpsertCardDto dto)
    {
        try
        {
            await cardService.CreateCardAsync(dto);
            return Ok();
        } 
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    // Debug
    [HttpGet("all")]
    public async Task<IActionResult> GetAllCards()
    {
        return Ok(await cardService.GetAllCardsAsync());
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllCardsForUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Ok(await cardService.GetCardsByUserIdAsync(userId!));
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCard([FromRoute] Guid id, [FromBody] UpsertCardDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await cardService.UpdateCardAsync(id, dto, userId!);
            return Ok();
        }
        catch  (Exception ex)
        {
            return BadRequest( new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCard([FromRoute] Guid id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await cardService.DeleteCardAsync(id, userId!);
            return Ok();
        } 
        catch (Exception ex)
        {
            return BadRequest( new { message = ex.Message });
        }
    }
}

