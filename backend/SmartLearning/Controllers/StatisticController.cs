using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Services;

namespace SmartLearning.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StatisticController(IStatisticService statisticService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetStatistics()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null) return Unauthorized();

        try
        {
            return Ok(await statisticService.GetStatisticsAsync(userId));
        }
        catch (Exception ex)
        {
            return BadRequest(new {message = ex.Message});
        } 
    }
}