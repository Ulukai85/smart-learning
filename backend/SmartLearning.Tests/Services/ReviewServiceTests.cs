using FluentAssertions;
using Moq;
using SmartLearning.DTOs;
using SmartLearning.Models;
using SmartLearning.Repositories;
using SmartLearning.Services;
using SmartLearning.SpacedRepetition;
using SmartLearning.Utils;

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

    #region GetDeckToReviewAsync Tests

    [Fact]
    public async Task GetDeckToReviewAsync_ShouldThrow_WhenDeckNotFound()
    {
        // Arrange
        deckRepo
            .Setup(r => r.GetDeckByIdAsync(defaultDeckId))
            .ReturnsAsync((Deck?)null);
        
        var reviewService = CreateReviewService();
        
        // Act
        var act = () => reviewService.GetDeckToReviewAsync(defaultDeckId, defaultUserId, 10, 10);
        
        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
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
    public async Task GetDeckToReviewAsync_ShouldClampNegativeLimits()
    {
        // Arrange
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

        cardRepo.Setup(r => r.GetDueCardsAsync(defaultDeckId, defaultUserId, 0, It.IsAny<DateTime>()))
            .ReturnsAsync(new List<CardToReviewDto>());

        cardRepo.Setup(r => r.GetNewCardsAsync(defaultDeckId, defaultUserId, 0))
            .ReturnsAsync(new List<CardToReviewDto>());

        var reviewService = CreateReviewService();

        // Act
        await reviewService.GetDeckToReviewAsync(defaultDeckId, defaultUserId, -10, -5);

        // Assert
        cardRepo.Verify(r => 
                r.GetDueCardsAsync(defaultDeckId, defaultUserId, 0, It.IsAny<DateTime>()),
                Times.Once);

        cardRepo.Verify(r =>
                r.GetNewCardsAsync(defaultDeckId, defaultUserId, 0),
                Times.Once);
    }

    [Fact]
    public async Task GetDeckToReviewAsync_ShouldReturnCorrectDeckData()
    {
        // Arrange
        const string deckName = "Test Deck";
        const int dueCount = 10;
        const int newCount = 5;

        deckRepo
            .Setup(r => r.GetDeckByIdAsync(defaultDeckId))
            .ReturnsAsync(new Deck
            {
                Id = defaultDeckId,
                Name = deckName,
                OwnerUserId = defaultUserId
            });

        cardRepo.Setup(r => r.CountDueCardsAsync(defaultDeckId, defaultUserId, It.IsAny<DateTime>()))
            .ReturnsAsync(dueCount);

        cardRepo.Setup(r => r.CountNewCardsAsync(defaultDeckId, defaultUserId))
            .ReturnsAsync(newCount);

        cardRepo.Setup(r => r.GetDueCardsAsync(defaultDeckId, defaultUserId, It.IsAny<int>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<CardToReviewDto>());

        cardRepo.Setup(r => r.GetNewCardsAsync(defaultDeckId, defaultUserId, It.IsAny<int>()))
            .ReturnsAsync(new List<CardToReviewDto>());

        var reviewService = CreateReviewService();

        // Act
        var result = await reviewService.GetDeckToReviewAsync(defaultDeckId, defaultUserId, 10, 10);

        // Assert
        result.Id.Should().Be(defaultDeckId);
        result.Name.Should().Be(deckName);
        result.DueCards.Should().Be(dueCount);
        result.NewCards.Should().Be(newCount);
        result.Cards.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDeckToReviewAsync_ShouldCombineDueAndNewCards()
    {
        // Arrange
        var dueCard = new CardToReviewDto { Id = Guid.NewGuid(), Front = "Front1", Back = "Back1" };
        var newCard = new CardToReviewDto { Id = Guid.NewGuid(), Front = "Front2", Back = "Back2" };

        deckRepo
            .Setup(r => r.GetDeckByIdAsync(defaultDeckId))
            .ReturnsAsync(new Deck
            {
                Id = defaultDeckId,
                Name = "Deck",
                OwnerUserId = defaultUserId
            });

        cardRepo.Setup(r => r.CountDueCardsAsync(defaultDeckId, defaultUserId, It.IsAny<DateTime>()))
            .ReturnsAsync(1);

        cardRepo.Setup(r => r.CountNewCardsAsync(defaultDeckId, defaultUserId))
            .ReturnsAsync(1);

        cardRepo.Setup(r => r.GetDueCardsAsync(defaultDeckId, defaultUserId, It.IsAny<int>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<CardToReviewDto> { dueCard });

        cardRepo.Setup(r => r.GetNewCardsAsync(defaultDeckId, defaultUserId, It.IsAny<int>()))
            .ReturnsAsync(new List<CardToReviewDto> { newCard });

        var reviewService = CreateReviewService();

        // Act
        var result = await reviewService.GetDeckToReviewAsync(defaultDeckId, defaultUserId, 10, 10);

        // Assert
        result.Cards.Should().HaveCount(2);
        result.Cards.Should().Contain(dueCard);
        result.Cards.Should().Contain(newCard);
    }

    #endregion

    #region HandleReviewTransactionAsync Tests - New Card Scenarios

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

        progressRepo.Verify(r => r.AddProgressAsync(It.IsAny<UserCardProgress>()), Times.Once);
        transactionRepo.Verify(r => r.AddXpTransactionAsync(It.IsAny<XpTransaction>()), Times.Once);
        reviewRepo.Verify(r => r.AddReviewLogAsync(It.IsAny<ReviewLog>()), Times.Once);
        reviewRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleReviewTransactionAsync_ForNewCard_ShouldUseDefaultStrategyType()
    {
        // Arrange
        var card = DefaultCard();
        var dto = new CreateReviewTransactionDto
        {
            CardId = defaultCardId,
            Grade = 2,
            StrategyType = null  // Not provided
        };
        
        SetupCommonReviewFlow(card, progress: null);
        
        var reviewService = CreateReviewService();

        // Act
        await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        spacedRepetitionFactory.Verify(f => f.GetStrategy("Anki"), Times.Once);
        progressRepo.Verify(r => r.AddProgressAsync(It.Is<UserCardProgress>(p => p.StrategyType == "Anki")), Times.Once);
    }

    [Fact]
    public async Task HandleReviewTransactionAsync_ForNewCard_ShouldUseProvidedStrategyType()
    {
        // Arrange
        var card = DefaultCard();
        var dto = new CreateReviewTransactionDto
        {
            CardId = defaultCardId,
            Grade = 2,
            StrategyType = "SM2"  // Custom strategy
        };
        
        SetupCommonReviewFlow(card, progress: null);
        spacedRepetitionFactory.Setup(f => f.GetStrategy("SM2"))
            .Returns(spacedRepetitionStrategy.Object);
        
        var reviewService = CreateReviewService();

        // Act
        await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        spacedRepetitionFactory.Verify(f => f.GetStrategy("SM2"), Times.Once);
        progressRepo.Verify(r => r.AddProgressAsync(It.Is<UserCardProgress>(p => p.StrategyType == "SM2")), Times.Once);
    }

    [Fact]
    public async Task HandleReviewTransactionAsync_ForNewCard_ShouldSetCreatedAtAndLastReviewedAt()
    {
        // Arrange
        var card = DefaultCard();
        var dto = DefaultDto();
        
        SetupCommonReviewFlow(card, progress: null);
        
        var reviewService = CreateReviewService();

        // Act
        await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        progressRepo.Verify(r => r.AddProgressAsync(It.Is<UserCardProgress>(p => 
            p.CreatedAt == fixedNow && 
            p.LastReviewedAt == fixedNow)), 
            Times.Once);
    }

    #endregion

    #region HandleReviewTransactionAsync Tests - Existing Card Scenarios

    [Fact]
    public async Task HandleReviewTransactionAsync_WhenProgressExists_ShouldNotAddProgress()
    {
        // Arrange
        var existingProgress = new UserCardProgress
        {
            UserId = defaultUserId,
            CardId = defaultCardId,
            CreatedAt = fixedNow.AddDays(-5),
            StrategyType = "Anki"
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

    [Fact]
    public async Task HandleReviewTransactionAsync_ShouldUpdateProgressDates()
    {
        // Arrange
        var existingProgress = new UserCardProgress
        {
            UserId = defaultUserId,
            CardId = defaultCardId,
            CreatedAt = fixedNow.AddDays(-5),
            LastReviewedAt = fixedNow.AddDays(-3),
            StrategyType = "Anki"
        };

        var card = DefaultCard();
        var dto = DefaultDto();
        
        SetupCommonReviewFlow(card, existingProgress);
        
        var reviewService = CreateReviewService();
        
        // Act
        await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);
        
        // Assert
        existingProgress.LastReviewedAt.Should().Be(fixedNow);
        existingProgress.UpdatedAt.Should().Be(fixedNow);
        existingProgress.NextReviewAt.Should().Be(fixedNow.AddDays(4));
    }

    #endregion

    #region HandleReviewTransactionAsync Tests - XP and Reinsertion

    [Fact]
    public async Task HandleReviewTransactionAsync_WhenNotReinserted_ShouldAwardXp()
    {
        // Arrange
        var card = DefaultCard();
        var dto = DefaultDto();
        
        SetupCommonReviewFlow(card, progress: null);
        spacedRepetitionStrategy.Setup(s => s.ShouldReinsert(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(false);
        
        var reviewService = CreateReviewService();

        // Act
        var result = await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        result.XpTransactions.Should().HaveCount(1);
        result.XpTransactions[0].Amount.Should().Be(5);
        result.XpTransactions[0].Reason.Should().Be("CardReview");
        transactionRepo.Verify(r => r.AddXpTransactionAsync(It.IsAny<XpTransaction>()), Times.Once);
    }

    [Fact]
    public async Task HandleReviewTransactionAsync_WhenReinserted_ShouldNotAwardXp()
    {
        // Arrange
        var card = DefaultCard();
        var dto = DefaultDto();
        
        SetupCommonReviewFlow(card, progress: null);
        spacedRepetitionStrategy.Setup(s => s.ShouldReinsert(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(true);
        
        var reviewService = CreateReviewService();

        // Act
        var result = await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        result.ReinsertCard.Should().BeTrue();
        result.XpTransactions.Should().HaveCount(0);
        transactionRepo.Verify(r => r.AddXpTransactionAsync(It.IsAny<XpTransaction>()), Times.Never);
    }

    #endregion

    #region HandleReviewTransactionAsync Tests - Deck Completion Bonus

    [Fact]
    public async Task HandleReviewTransactionAsync_WhenDeckCompleted_ShouldAwardDeckBonus()
    {
        // Arrange
        var card = DefaultCard();
        var dto = DefaultDto();
        
        SetupCommonReviewFlow(card, progress: null);
        
        // No more due or new cards after this review
        cardRepo.Setup(r => r.CountDueCardsAsync(card.DeckId, defaultUserId, fixedNow))
            .ReturnsAsync(0);
        cardRepo.Setup(r => r.CountNewCardsAsync(card.DeckId, defaultUserId))
            .ReturnsAsync(0);
        
        var reviewService = CreateReviewService();

        // Act
        var result = await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        result.XpTransactions.Should().HaveCount(2);
        result.XpTransactions.Should().ContainSingle(x => x.Reason == "CardReview" && x.Amount == 5);
        result.XpTransactions.Should().ContainSingle(x => x.Reason == "DeckCompleted" && x.Amount == 20);
    }

    [Fact]
    public async Task HandleReviewTransactionAsync_WhenDeckNotCompleted_ShouldNotAwardDeckBonus()
    {
        // Arrange
        var card = DefaultCard();
        var dto = DefaultDto();
        
        SetupCommonReviewFlow(card, progress: null);
        
        // Still have cards remaining
        cardRepo.Setup(r => r.CountDueCardsAsync(card.DeckId, defaultUserId, fixedNow))
            .ReturnsAsync(2);
        cardRepo.Setup(r => r.CountNewCardsAsync(card.DeckId, defaultUserId))
            .ReturnsAsync(1);
        
        var reviewService = CreateReviewService();

        // Act
        var result = await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        result.XpTransactions.Should().HaveCount(1);
        result.XpTransactions[0].Reason.Should().Be("CardReview");
    }

    [Fact]
    public async Task HandleReviewTransactionAsync_WhenDeckCompletedButCardReinserted_ShouldStillAwardDeckBonus()
    {
        // Arrange
        var card = DefaultCard();
        var dto = DefaultDto();
        
        SetupCommonReviewFlow(card, progress: null);
        spacedRepetitionStrategy.Setup(s => s.ShouldReinsert(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(true);  // Card is reinserted, so no normal XP
        
        cardRepo.Setup(r => r.CountDueCardsAsync(card.DeckId, defaultUserId, fixedNow))
            .ReturnsAsync(0);
        cardRepo.Setup(r => r.CountNewCardsAsync(card.DeckId, defaultUserId))
            .ReturnsAsync(0);
        
        var reviewService = CreateReviewService();

        // Act
        var result = await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        result.XpTransactions.Should().HaveCount(1);
        result.XpTransactions[0].Reason.Should().Be("DeckCompleted");
    }

    #endregion

    #region HandleReviewTransactionAsync Tests - Review Log and Persistence

    [Fact]
    public async Task HandleReviewTransactionAsync_ShouldCreateReviewLog()
    {
        // Arrange
        var card = DefaultCard();
        var dto = DefaultDto(grade: 3);
        
        SetupCommonReviewFlow(card, progress: null);
        
        var reviewService = CreateReviewService();

        // Act
        await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        reviewRepo.Verify(r => r.AddReviewLogAsync(It.Is<ReviewLog>(log =>
            log.UserId == defaultUserId &&
            log.CardId == defaultCardId &&
            log.Grade == 3 &&
            log.ReviewedAt == fixedNow &&
            log.StrategyType == "Anki"
        )), Times.Once);
    }

    [Fact]
    public async Task HandleReviewTransactionAsync_ShouldSaveChanges()
    {
        // Arrange
        var card = DefaultCard();
        var dto = DefaultDto();
        
        SetupCommonReviewFlow(card, progress: null);
        
        var reviewService = CreateReviewService();

        // Act
        await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        reviewRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region HandleReviewTransactionAsync Tests - Result Data

    [Fact]
    public async Task HandleReviewTransactionAsync_ShouldReturnCorrectResultData()
    {
        // Arrange
        var card = DefaultCard();
        var dto = DefaultDto();
        const int expectedDueCount = 5;
        const int expectedNewCount = 3;
        
        SetupCommonReviewFlow(card, progress: null);
        cardRepo.Setup(r => r.CountDueCardsAsync(card.DeckId, defaultUserId, fixedNow))
            .ReturnsAsync(expectedDueCount);
        cardRepo.Setup(r => r.CountNewCardsAsync(card.DeckId, defaultUserId))
            .ReturnsAsync(expectedNewCount);
        
        var reviewService = CreateReviewService();

        // Act
        var result = await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        result.ReviewedCardId.Should().Be(defaultCardId);
        result.UpdatedDueCount.Should().Be(expectedDueCount);
        result.UpdatedNewCount.Should().Be(expectedNewCount);
    }

    [Fact]
    public async Task HandleReviewTransactionAsync_ShouldPassGradeToSpacedRepetitionStrategy()
    {
        // Arrange
        var card = DefaultCard();
        var dto = new CreateReviewTransactionDto
        {
            CardId = defaultCardId,
            Grade = 4
        };
        
        SetupCommonReviewFlow(card, progress: null);
        
        var reviewService = CreateReviewService();

        // Act
        await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        spacedRepetitionStrategy.Verify(s => 
            s.CalculateNextReview(4, fixedNow, It.IsAny<string>()), 
            Times.Once);
    }

    #endregion

    #region HandleReviewTransactionAsync Tests - Authorization and Validation

    [Fact]
    public async Task HandleReviewTransactionAsync_ShouldThrow_WhenCardNotFound()
    {
        // Arrange
        cardRepo.Setup(r => r.GetCardByIdAsync(defaultCardId))
            .ReturnsAsync((Card?)null);

        var reviewService = CreateReviewService();
        var dto = DefaultDto();

        // Act
        var act = () => reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task HandleReviewTransactionAsync_ShouldThrow_WhenCardNotOwnedByUser()
    {
        // Arrange
        var card = new Card
        {
            Id = defaultCardId,
            DeckId = defaultDeckId,
            Deck = new Deck { OwnerUserId = "user-2" }  // Different owner
        };

        cardRepo.Setup(r => r.GetCardByIdAsync(defaultCardId))
            .ReturnsAsync(card);

        var reviewService = CreateReviewService();
        var dto = DefaultDto();

        // Act
        var act = () => reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task HandleReviewTransactionAsync_ShouldThrow_WhenCardDeckIsNull()
    {
        // Arrange
        var card = new Card
        {
            Id = defaultCardId,
            DeckId = defaultDeckId,
            Deck = null!  // No deck
        };

        cardRepo.Setup(r => r.GetCardByIdAsync(defaultCardId))
            .ReturnsAsync(card);

        var reviewService = CreateReviewService();
        var dto = DefaultDto();

        // Act
        var act = () => reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }

    #endregion

    #region HandleReviewTransactionAsync Tests - Different Grade Values

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task HandleReviewTransactionAsync_ShouldWorkWithVariousGrades(int grade)
    {
        // Arrange
        var card = DefaultCard();
        var dto = new CreateReviewTransactionDto
        {
            CardId = defaultCardId,
            Grade = grade
        };
        
        SetupCommonReviewFlow(card, progress: null);
        
        var reviewService = CreateReviewService();

        // Act
        var result = await reviewService.HandleReviewTransactionAsync(defaultUserId, dto);

        // Assert
        result.Should().NotBeNull();
        spacedRepetitionStrategy.Verify(s => s.CalculateNextReview(grade, It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
    }

    #endregion
}