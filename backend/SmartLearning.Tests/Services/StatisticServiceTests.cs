using FluentAssertions;
using Moq;
using SmartLearning.Repositories;
using SmartLearning.Services;

namespace SmartLearning.Tests.Services;

public class StatisticServiceTests
{
    private readonly Mock<IReviewRepository> reviewRepo = new();
    private readonly Mock<ITransactionRepository> transactionRepo = new();
    
    private readonly string defaultUserId = "user-1";
    
    private StatisticService CreateStatisticService()
    {
        return new StatisticService(
            reviewRepo.Object,
            transactionRepo.Object);
    }
    
    [Fact]
    public async Task GetStreakDataAsync_ShouldReturnLongestStreak()
    {
        // Arrange
        var reviewDates = new List<DateTime>
        {
            new(2024, 12, 28),
            new(2024, 12, 29),
            new(2024, 12, 30),
            new(2024, 12, 31),
            new(2025, 1, 1)
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(reviewDates.Select(DateOnly.FromDateTime).ToHashSet());

        var reviewService = CreateStatisticService();

        // Act
        var streakData = await reviewService.GetStreakDataAsync(defaultUserId);

        // Assert
        streakData.LongestStreak.Should().Be(5);
    }
    
    [Fact]
    public async Task GetStreakDataAsync_NoReviews_ReturnsZeroStreaks()
    {
        // Arrange
        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync("user1"))
            .ReturnsAsync([]);

        var reviewService = CreateStatisticService();
        
        // Act
        var result = await reviewService.GetStreakDataAsync("user1");

        // Assert
        result.CurrentStreak.Should().Be(0);
        result.LongestStreak.Should().Be(0);
    }
    
    [Fact]
    public async Task GetStreakDataAsync_ThreeConsecutiveDays_ReturnsCorrectStreak()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var dates = new HashSet<DateOnly>
        {
            today.AddDays(-2),
            today.AddDays(-1),
            today
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync("user1"))
            .ReturnsAsync(dates);
        
        var reviewService = CreateStatisticService();

        var result = await reviewService.GetStreakDataAsync("user1");

        result.LongestStreak.Should().Be(3);
        result.CurrentStreak.Should().Be(3);
    }
    
    [Fact]
    public async Task GetStreakDataAsync_NoReviewToday_ButYesterdayCounts()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var dates = new HashSet<DateOnly>
        {
            today.AddDays(-3),
            today.AddDays(-2),
            today.AddDays(-1)
        };
        
        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync("user1"))
            .ReturnsAsync(dates);

        var reviewService = CreateStatisticService();
        
        var result = await reviewService.GetStreakDataAsync("user1");

        result.LongestStreak.Should().Be(3);
        result.CurrentStreak.Should().Be(3);
    }
    
    [Fact]
    public async Task GetStreakDataAsync_LongestDifferentFromCurrent_ReturnsCorrectValues()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var dates = new HashSet<DateOnly>
        {
            today.AddDays(-10),
            today.AddDays(-9),
            today.AddDays(-8), // longest = 3
            today.AddDays(-1)  // current = 1
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync("user1"))
            .ReturnsAsync(dates);

        var reviewService = CreateStatisticService();
        
        var result = await reviewService.GetStreakDataAsync("user1");

        result.LongestStreak.Should().Be(3);
        result.CurrentStreak.Should().Be(1);
    }
    
}