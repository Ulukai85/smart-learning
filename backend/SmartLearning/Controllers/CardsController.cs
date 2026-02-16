using System.Security.Authentication;
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
    
    [HttpGet]
    public async Task<IActionResult> GetAllCards()
    {
        return Ok(await cardService.GetAllCardsAsync());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCard([FromRoute] Guid id, [FromBody] UpsertCardDto dto)
    {
        try
        {
            await cardService.UpdateCardAsync(id, dto);
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
            await cardService.DeleteCardAsync(id);
            return Ok();
        } catch (Exception ex)
        {
            return BadRequest( new { message = ex.Message });
        }
    }
}

