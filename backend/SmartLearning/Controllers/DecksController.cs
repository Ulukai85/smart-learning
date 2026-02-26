
using System.Security.Claims;
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Ok(await deckService.CreateDeckAsync(dto, userId));
        } 
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Debug
    [HttpGet("all")]
    public async Task<IActionResult> GetAllDecks()
    {
        return Ok(await deckService.GetAllDecksAsync());
    }
    
    [HttpGet]
    public async Task<IActionResult> GetDecksForUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Ok(await deckService.GetDecksByUserIdAsync(userId!));
    }
    
    [HttpGet("published")]
    public async Task<IActionResult> GetPublishedDecks()
    {
        return Ok(await deckService.GetPublishedDecksAsync());
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

    [HttpGet("summary")]
    public async Task<IActionResult> GetDeckSummary()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var summaries = await deckService.GetDeckSummariesByUserIdAsync(userId!);
            return Ok(summaries);
        } 
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> PublishDeck([FromRoute] Guid id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await deckService.SetIsPublishedAsync(true, userId!, id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPatch("{id:guid}/unpublish")]
    public async Task<IActionResult> UnpublishDeck([FromRoute] Guid id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await deckService.SetIsPublishedAsync(false, userId!, id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

