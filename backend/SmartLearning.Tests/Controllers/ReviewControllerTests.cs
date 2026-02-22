using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartLearning.Controllers;
using SmartLearning.DTOs;
using SmartLearning.Services;

namespace SmartLearning.Tests.Controllers;

public class ReviewControllerTests
{
    [Fact]
    public async Task GetCardsToReview_ShouldReturnUnauthorized_WhenNoUser()
    {
        var mockService = new Mock<IReviewService>();
        var controller = new ReviewController(mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.GetCardsToReview(Guid.NewGuid());
        
        result.Should().BeOfType<UnauthorizedResult>();
    }
    
    [Fact]
    public async Task GetCardsToReview_ShouldReturnOk()
    {
        var deckId = Guid.NewGuid();
        var userId = "user-1";

        var mockService = new Mock<IReviewService>();
        mockService
            .Setup(s => s.GetDeckToReviewAsync(deckId, userId, 20, 10))
            .ReturnsAsync(new DeckToReviewDto
            {
                Id = deckId,
                Name = "Test Deck"
            });

        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, userId)
        ]));
        
        var controller = new ReviewController(mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            }
        };
        
        var result = await controller.GetCardsToReview(deckId);

        result.Should().BeOfType<OkObjectResult>();
    }
}