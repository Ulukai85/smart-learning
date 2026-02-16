using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartLearning.DTOs;
using SmartLearning.Models;
using SmartLearning.Repositories;
using SmartLearning.Services;

namespace SmartLearning.Tests.Services;

public class CardServiceTests
{
   [Fact]
   public async Task CreateCard_ShouldPersistCard()
   {
      // Arrange
      var options = new DbContextOptionsBuilder<AppDbContext>()
         .UseInMemoryDatabase(databaseName: "TestDb")
         .Options;
      
      await using var context = new AppDbContext(options);
      var service = new CardService(new CardRepository(context));

      var card = new UpsertCardDto
      {
         Front = "Frage",
         Back = "Antwort",
         DeckId = Guid.NewGuid()
      };
      
      // Act
      var createdCard = await service.CreateCardAsync(card);
      
      // Assert
      var fetchedCard = await context.Cards.FindAsync([createdCard.Id], TestContext.Current.CancellationToken);
      fetchedCard.Should().NotBeNull();
      fetchedCard.Id.Should().Be(createdCard.Id);
   }
}