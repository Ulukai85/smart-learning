
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.DTOs;
using SmartLearning.Services;

namespace SmartLearning.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DecksController(IDeckService deckService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateDeck ([FromBody] UpsertDeckDto dto)
    {
        try
        {
            var userId = User.Claims.First(c => c.Type == "UserId").Value;
            return Ok(await deckService.CreateDeckAsync(dto, userId));
        } 
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDecks()
    {
        return Ok(await deckService.GetAllDecksAsync());
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateDeck([FromRoute] Guid id, [FromBody] UpsertDeckDto dto)
    {
        try
        {
            await deckService.UpdateDeckAsync(id, dto);
            return Ok();
        }
        catch  (Exception ex)
        {
            return BadRequest( new { message = ex.Message });
        }
    }
}

