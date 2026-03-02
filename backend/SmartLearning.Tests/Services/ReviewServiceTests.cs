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
    private readonly Mock<ISpacedRepetitionFactory> spacedRepetitionFactory = new();
    private readonly Mock<ISpacedRepetitionStrategy> spacedRepetitionStrategy = new();
    private readonly Mock<ITimeProvider> timeProvider = new();
    
    private readonly DateTime fixedNow = new(2025, 1, 1);
    private readonly string defaultUserId = "user-1";
    private readonly Guid defaultCardId = Guid.NewGuid();
    private readonly Guid defaultDeckId = Guid.NewGuid();

    private Card DefaultCard() => new()
    {
        Id = defaultCardId,
        DeckId = defaultDeckId,
        Deck = new Deck { OwnerUserId = defaultUserId }
    };

    private CreateReviewTransactionDto DefaultDto(int grade = 2) => new()
    {
        CardId = defaultCardId,
        Grade = grade
    };

    private void SetupCommonReviewFlow(Card card, UserCardProgress? progress = null)
    {
        cardRepo.Setup(r => r.GetCardByIdAsync(card.Id))
            .ReturnsAsync(card);

        progressRepo.Setup(r => r.GetProgressAsync(defaultUserId, card.Id))
            .ReturnsAsync(progress);

        timeProvider.Setup(t => t.UtcNow)
            .Returns(fixedNow);

        spacedRepetitionStrategy.Setup(s => s.CalculateNextReview(It.IsAny<int>(), fixedNow, It.IsAny<string>()))
            .Returns(fixedNow.AddDays(4));

        spacedRepetitionStrategy.Setup(s => s.ShouldReinsert(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(false);

        spacedRepetitionFactory.Setup(f => f.GetStrategy(It.IsAny<string>()))
            .Returns(spacedRepetitionStrategy.Object);
        
        cardRepo.Setup(r => r.CountDueCardsAsync(It.IsAny<Guid>(), defaultUserId, fixedNow))
            .ReturnsAsync(5);

        cardRepo.Setup(r => r.CountNewCardsAsync(It.IsAny<Guid>(), defaultUserId))
            .ReturnsAsync(3);
    }

    private ReviewService CreateReviewService()
    {
        return new ReviewService(
            cardRepo.Object,
            deckRepo.Object,
            reviewRepo.Object,
            transactionRepo.Object,
            progressRepo.Object,
            spacedRepetitionFactory.Object,
            timeProvider.Object);
    }

    [Fact]
    public async Task GetDeckToReviewAsync_ShouldThrow_WhenDeckNotOwnedByUser()
    {
        // Arrange
        deckRepo
            .Setup(r => r.GetDeckByIdAsync(defaultDeckId))
            .ReturnsAsync(new Deck
            {
                Id = defaultDeckId,
                Name = "Test Deck",
                OwnerUserId = "user-2"
            });
        
        var reviewService = CreateReviewService();
        
        // Act
        var act = () => reviewService.GetDeckToReviewAsync(defaultDeckId, defaultUserId, 10, 10);
        
        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
    
    [Fact]
    public async Task GetDeckToReviewAsync_ShouldClampLimits()
    {
        deckRepo
            .Setup(r => r.GetDeckByIdAsync(defaultDeckId))
            .ReturnsAsync(new Deck
            {
                Id = defaultDeckId,
                Name = "Deck",
                OwnerUserId = defaultUserId
            });

        cardRepo.Setup(r => r.CountDueCardsAsync(It.IsAny<Guid>(), defaultUserId, It.IsAny<DateTime>()))
            .ReturnsAsync(0);

        cardRepo.Setup(r => r.CountNewCardsAsync(defaultDeckId, defaultUserId))
            .ReturnsAsync(0);

        cardRepo.Setup(r => r.GetDueCardsAsync(defaultDeckId, defaultUserId, 50, It.IsAny<DateTime>()))
            .ReturnsAsync(new List<CardToReviewDto>());

        cardRepo.Setup(r => r.GetNewCardsAsync(defaultDeckId, defaultUserId, 20))
            .ReturnsAsync(new List<CardToReviewDto>());

        var reviewService = CreateReviewService();

        await reviewService.GetDeckToReviewAsync(defaultDeckId, defaultUserId, 999, 999);

        cardRepo.Verify(r => 
                r.GetDueCardsAsync(defaultDeckId, defaultUserId, 50, It.IsAny<DateTime>()),
                Times.Once);

        cardRepo.Verify(r =>
                r.GetNewCardsAsync(defaultDeckId, defaultUserId, 20),
                Times.Once);
    }
    
    [Fact]
    public async Task HandleReviewTransactionAsync_Should_CreateProgressAndReturnResult()
    {
        // Arrange
        var card = DefaultCard();
        var dto = DefaultDto();
        
        SetupCommonReviewFlow(card, progress: null);
        
        var reviewService = CreateReviewService();

        // Act
        var result = await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        result.WasNew.Should().BeTrue();
        result.ReinsertCard.Should().BeFalse();
        result.NextReviewAt.Should().Be(fixedNow.AddDays(4));
        result.XpAmount.Should().Be(5);

        progressRepo.Verify(r => r.AddProgressAsync(It.IsAny<UserCardProgress>()), Times.Once);
        transactionRepo.Verify(r => r.AddXpTransactionAsync(It.IsAny<XpTransaction>()), Times.Once);
        reviewRepo.Verify(r => r.AddReviewLogAsync(It.IsAny<ReviewLog>()), Times.Once);
        reviewRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleReviewTransactionAsync_WhenProgressExists_ShouldNotAddProgress()
    {
        // Arrange
        var existingProgress = new UserCardProgress
        {
            UserId = defaultUserId,
            CardId = defaultCardId,
            CreatedAt = fixedNow.AddDays(-5)
        };

        var card = DefaultCard();
        var dto = DefaultDto();
        
        SetupCommonReviewFlow(card, existingProgress);
        
        var reviewService = CreateReviewService();
        
        // Act
        var result = await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);
        
        // Assert
        result.WasNew.Should().BeFalse();
        
        progressRepo.Verify(r => r.AddProgressAsync(It.IsAny<UserCardProgress>()), Times.Never);
        reviewRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        existingProgress.NextReviewAt.Should().Be(fixedNow.AddDays(4));
    }
}