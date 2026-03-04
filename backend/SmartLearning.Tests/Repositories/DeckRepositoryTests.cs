using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Models;
using SmartLearning.Repositories;

namespace SmartLearning.Tests.Services;

public class DeckRepositoryTests
{
    private readonly string userA = "userA";
    private readonly string userB = "userB";
    
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
    
    private Card createCard(Guid deckId) => new()
    {
        Id = Guid.NewGuid(),
        Front = "Front",
        Back = "Back",
        DeckId = deckId,
    };

    private async Task SeedWithoutForkAsync(AppDbContext context)
    {
        var rootPublished = new Deck
        {
            Id = Guid.NewGuid(),
            OwnerUserId = userA,
            Name = "Root Published",
            IsPublished = true,
        };
        
        context.Decks.Add(rootPublished);
        context.Cards.AddRange(createCard(rootPublished.Id), createCard(rootPublished.Id), createCard(rootPublished.Id));
        await context.SaveChangesAsync();
    }
    
    private async Task SeedWithForkAsync(AppDbContext context)
    {
        var rootPublished = new Deck
        {
            Id = Guid.NewGuid(),
            OwnerUserId = userA,
            Name = "Root Published",
            IsPublished = true,
        };
        
        var forkedDeck = new Deck
        {
            Id = Guid.NewGuid(),
            OwnerUserId = userB,
            Name = "Forked",
            IsPublished = true,
            SourceDeckId = rootPublished.Id
        };

        context.Decks.AddRange(rootPublished, forkedDeck);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task Should_Return_Only_Public_Root_Decks_Not_Owned_By_User()
    {
        // Arrange
        var context = CreateContext();
        await SeedWithoutForkAsync(context);

        var repo = new DeckRepository(context);

        // Act
        var result = await repo.GetPublishedDecksAsync(userB);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Root Published");
    }

    [Fact]
    public async Task Should_Not_Return_Forked_Deck_If_User_Already_Forked()
    {
        var context = CreateContext();
        await SeedWithForkAsync(context);

        var repo = new DeckRepository(context);

        var result = await repo.GetPublishedDecksAsync(userB);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Map_TotalCards_Correctly()
    {
        var context = CreateContext();
        await SeedWithoutForkAsync(context);

        var repo = new DeckRepository(context);

        var result = await repo.GetPublishedDecksAsync(userB);
        result.Should().HaveCount(1);
        result.First().TotalCards.Should().Be(3);
    }
}