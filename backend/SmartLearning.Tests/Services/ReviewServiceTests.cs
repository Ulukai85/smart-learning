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
    private readonly Mock<IProgressRepository> progressRepo = new();
    private readonly Mock<ISpacedRepetition> spacedRepetition = new();
    private readonly Mock<ITimeProvider> timeProvider = new();

    private ReviewService CreateReviewService()
    {
        return new ReviewService(
            cardRepo.Object,
            deckRepo.Object,
            reviewRepo.Object,
            transactionRepo.Object,
            progressRepo.Object,
            spacedRepetition.Object,
            timeProvider.Object);
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
    
    [Fact]
    public async Task HandleReviewTransactionAsync_Should_CreateProgressAndReturnResult()
    {
        // Arrange
        var userId = "user1";
        var cardId = Guid.NewGuid();
        var now = new DateTime(2025, 1, 1);

        var card = new Card
        {
            Id = cardId,
            DeckId = Guid.NewGuid(),
            Deck = new Deck { OwnerUserId = userId }
        };

        var dto = new CreateReviewTransactionDto
        {
            CardId = cardId,
            Grade = 2
        };
        
        cardRepo.Setup(r => r.GetCardByIdAsync(cardId))
            .ReturnsAsync(card);
        cardRepo.Setup(r => r.CountDueCardsAsync(It.IsAny<Guid>(), userId, now))
            .ReturnsAsync(5);
        cardRepo.Setup(r => r.CountNewCardsAsync(It.IsAny<Guid>(), userId))
            .ReturnsAsync(3);

        progressRepo.Setup(r => r.GetProgressAsync(userId, cardId))
            .ReturnsAsync((UserCardProgress?)null);
        
        timeProvider.Setup(t => t.UtcNow).Returns(now);
        
        spacedRepetition.Setup(s => s.CalculateNextReview(dto.Grade, now))
            .Returns(now.AddDays(4));
        spacedRepetition.Setup(s => s.ShouldReinsert(dto.Grade))
            .Returns(false);
        
        var reviewService = CreateReviewService();

        // Act
        var result = await reviewService.HandleReviewTransactionAsync(userId, dto);

        // Assert
        result.WasNew.Should().BeTrue();
        result.ReinsertCard.Should().BeFalse();
        result.NextReviewAt.Should().Be(now.AddDays(4));
        result.XpAmount.Should().Be(5);

        progressRepo.Verify(r => r.AddProgressAsync(It.IsAny<UserCardProgress>()), Times.Once);
        transactionRepo.Verify(r => r.AddXpTransactionAsync(It.IsAny<XpTransaction>()), Times.Once);
        reviewRepo.Verify(r => r.AddReviewLogAsync(It.IsAny<ReviewLog>()), Times.Once);
        reviewRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}