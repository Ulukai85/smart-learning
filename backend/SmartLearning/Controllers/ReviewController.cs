using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Services;

namespace SmartLearning.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReviewController(
    IDeckService deckService,
    IReviewService reviewService) : ControllerBase
{
    [HttpGet("deck/{deckId:guid}")]
    public async Task<IActionResult> GetCardsToReview(
        Guid deckId,
        [FromQuery] int dueLimit = 20,
        [FromQuery] int newLimit = 10)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null) return Unauthorized();

        try
        {
            var batch = await reviewService.GetCardsToReviewAsync(deckId, userId, dueLimit, newLimit);
            return Ok(batch);
        }
        catch  (Exception ex)
        {
            return BadRequest(new {message = ex.Message});
        }
    }
}