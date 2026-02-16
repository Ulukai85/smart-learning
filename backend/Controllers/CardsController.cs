using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.DTOs;
using SmartLearning.Services;

namespace SmartLearning.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardController(ICardService cardService) : ControllerBase
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
}

