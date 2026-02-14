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
    public async Task<IActionResult> CreateCard([FromBody] CreateCardDto dto)
    {
        try
        {
            return Ok(await cardService.CreateCardAsync(dto));
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
}

