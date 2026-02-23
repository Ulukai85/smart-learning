using FluentAssertions;
using Moq;
using SmartLearning.DTOs;
using SmartLearning.Models;
using SmartLearning.Repositories;
using SmartLearning.Services;

namespace SmartLearning.Tests.Services;

public class ReviewServiceTests
{
    private readonly Mock<ICardRepository> cardRepo = new();
    private readonly Mock<IDeckRepository> deckRepo = new();
    private readonly Mock<IReviewRepository> reviewRepo = new();
    private readonly Mock<ITransactionRepository> transactionRepo = new();

    private ReviewService CreateReviewService()
    {
        return new ReviewService(
            cardRepo.Object,
            deckRepo.Object,
            transactionRepo.Object,
            reviewRepo.Object);  
    } 

    [Fact]
    public async Task GetDeckToReviewAsync_ShouldThrow_WhenDeckNotOwnedByUser()
    {
        // Arrange
        var deckId = Guid.NewGuid();
        var userId = "user-1";

        deckRepo
            .Setup(r => r.GetDeckByIdAsync(deckId))
            .ReturnsAsync(new Deck
            {
                Id = deckId,
                Name = "Test Deck",
                OwnerUserId = "user-2"
            });
        
        var reviewService = CreateReviewService();
        
        // Act
        var act = () => reviewService.GetDeckToReviewAsync(deckId, userId, 10, 10);
        
        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
    
    [Fact]
    public async Task GetDeckToReviewAsync_ShouldClampLimits()
    {
        var deckId = Guid.NewGuid();
        var userId = "user-1";

        deckRepo
            .Setup(r => r.GetDeckByIdAsync(deckId))
            .ReturnsAsync(new Deck
            {
                Id = deckId,
                Name = "Deck",
                OwnerUserId = userId
            });

        cardRepo.Setup(r => r.CountDueCardsAsync(It.IsAny<Guid>(), userId, It.IsAny<DateTime>()))
            .ReturnsAsync(0);

        cardRepo.Setup(r => r.CountNewCardsAsync(deckId, userId))
            .ReturnsAsync(0);

        cardRepo.Setup(r => r.GetDueCardsAsync(deckId, userId, 50, It.IsAny<DateTime>()))
            .ReturnsAsync(new List<CardToReviewDto>());

        cardRepo.Setup(r => r.GetNewCardsAsync(deckId, userId, 20))
            .ReturnsAsync(new List<CardToReviewDto>());

        var reviewService = CreateReviewService();

        await reviewService.GetDeckToReviewAsync(deckId, userId, 999, 999);

        cardRepo.Verify(r => 
                r.GetDueCardsAsync(deckId, userId, 50, It.IsAny<DateTime>()),
                Times.Once);

        cardRepo.Verify(r =>
                r.GetNewCardsAsync(deckId, userId, 20),
                Times.Once);
    }
}