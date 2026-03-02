using FluentAssertions;
using Microsoft.ApplicationInsights;
using Moq;
using SmartLearning.DTOs;
using SmartLearning.Models;
using SmartLearning.Repositories;
using SmartLearning.Services;

namespace SmartLearning.Tests.Services;

public class DeckServiceTests
{
    private readonly Mock<IDeckRepository> repoMock;
    private readonly DeckService deckService;
    
    public DeckServiceTests()
    {
        repoMock = new Mock<IDeckRepository>();
        deckService = new DeckService(repoMock.Object);
    }

    private ForkDeckDto CreateForkData()
    {
        return new ForkDeckDto
        {
            Name = "Original",
            Description = "Original",
            Cards = new List<ForkCardDto>
            {
                new ForkCardDto { Front = "Q1", Back = "A1" },
                new ForkCardDto { Front = "Q2", Back = "A2" },
            }
        };
    }
    
    [Fact]
    public async Task ForkDeckAsync_Should_Throw_When_Deck_Not_Found()
    {
        var deckId = Guid.NewGuid();
        var userId = "user1";

        repoMock.Setup(r => r.GetForkingDataByDeckId(deckId))!
            .ReturnsAsync((ForkDeckDto?)null);

        Func<Task> act = async () => 
            await deckService.ForkDeckAsync(deckId, userId);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Deck not found");

        repoMock.Verify(r => r.AddDeckWithCardsAsync(It.IsAny<Deck>(), It.IsAny<List<Card>>()), Times.Never);
        repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    
    [Fact]
    public async Task ForkDeckAsync_Should_Create_New_Deck_And_Copy_Cards()
    {
        var deckId = Guid.NewGuid();
        var userId = "user1";
        var forkData = CreateForkData();

        repoMock.Setup(r => r.GetForkingDataByDeckId(deckId))
            .ReturnsAsync(forkData);

        Deck? capturedDeck = null;
        List<Card>? capturedCards = null;

        repoMock.Setup(r => r.AddDeckWithCardsAsync(
                It.IsAny<Deck>(), 
                It.IsAny<List<Card>>()))
            .Callback<Deck, List<Card>>((deck, cards) =>
            {
                capturedDeck = deck;
                capturedCards = cards;
            })
            .Returns(Task.CompletedTask);

        repoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var newDeckId = await deckService.ForkDeckAsync(deckId, userId);

        // Assert
        capturedDeck.Should().NotBeNull();
        capturedCards.Should().NotBeNull();

        capturedDeck!.Id.Should().Be(newDeckId);
        capturedDeck.OwnerUserId.Should().Be(userId);
        capturedDeck.SourceDeckId.Should().Be(deckId);
        capturedDeck.Name.Should().Be("Original (forked)");
        capturedDeck.Description.Should().Be("Original");

        capturedCards!.Should().HaveCount(2);

        capturedCards.Should().AllSatisfy(card =>
        {
            card.Id.Should().NotBeEmpty();
            card.DeckId.Should().Be(newDeckId);
            card.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            card.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        });

        capturedCards.Select(c => c.Front)
            .Should().BeEquivalentTo(["Q1", "Q2"]);

        capturedCards.Select(c => c.Back)
            .Should().BeEquivalentTo(["A1", "A2"]);

        repoMock.Verify(r => r.AddDeckWithCardsAsync(It.IsAny<Deck>(), It.IsAny<List<Card>>()), Times.Once);
        repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task ForkDeckAsync_Should_Handle_Deck_With_No_Cards()
    {
        var deckId = Guid.NewGuid();
        var userId = "user1";

        var forkData = new ForkDeckDto
        {
            Name = "Empty Deck",
            Description = "No cards",
            Cards = []
        };

        repoMock.Setup(r => r.GetForkingDataByDeckId(deckId))
            .ReturnsAsync(forkData);

        List<Card>? capturedCards = null;

        repoMock.Setup(r => r.AddDeckWithCardsAsync(
                It.IsAny<Deck>(), 
                It.IsAny<List<Card>>()))
            .Callback<Deck, List<Card>>((_, cards) =>
            {
                capturedCards = cards;
            })
            .Returns(Task.CompletedTask);

        repoMock.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        await deckService.ForkDeckAsync(deckId, userId);

        capturedCards.Should().NotBeNull();
        capturedCards.Should().BeEmpty();
    }
    
    [Fact]
    public async Task Should_Call_Repository_And_Return_Result()
    {
        // Arrange
        var userId = "user1";

        var expected = new List<DeckDto>
        {
            new DeckDto
            {
                Id = Guid.NewGuid(),
                OwnerUserId = "owner",
                Name = "Deck 1",
                Description = "Desc",
                TotalCards = 5
            }
        };

        var repoMock = new Mock<IDeckRepository>();

        repoMock.Setup(r => r.GetPublishedDecksAsync(userId))
            .ReturnsAsync(expected);

        var service = new DeckService(repoMock.Object);

        // Act
        var result = await service.GetPublishedDecksAsync(userId);

        // Assert
        result.Should().BeEquivalentTo(expected);

        repoMock.Verify(r => r.GetPublishedDecksAsync(userId), Times.Once);
    }

    [Fact]
    public async Task Should_Return_Empty_List_When_Repo_Returns_Empty()
    {
        var repoMock = new Mock<IDeckRepository>();

        repoMock.Setup(r => r.GetPublishedDecksAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<DeckDto>());

        var service = new DeckService(repoMock.Object);

        var result = await service.GetPublishedDecksAsync("user");

        result.Should().BeEmpty();
    }
}