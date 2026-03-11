using FluentAssertions;
using Moq;
using SmartLearning.DTOs;
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

    #region GetStreakDataAsync Tests

    [Fact]
    public async Task GetStreakDataAsync_ShouldThrow_WhenUserIdIsNull()
    {
        // Arrange
        var statisticService = CreateStatisticService();

        // Act
        var act = () => statisticService.GetStreakDataAsync(null!);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetStreakDataAsync_NoReviews_ReturnsZeroStreaks()
    {
        // Arrange
        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync([]);

        var statisticService = CreateStatisticService();
        
        // Act
        var result = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        result.CurrentStreak.Should().Be(0);
        result.LongestStreak.Should().Be(0);
        result.ReviewDates.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStreakDataAsync_SingleReview_ReturnsSingleStreak()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dates = new HashSet<DateOnly> { today };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(dates);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        result.LongestStreak.Should().Be(1);
        result.CurrentStreak.Should().Be(1);
        result.ReviewDates.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetStreakDataAsync_FiveConsecutiveDays_ReturnsCorrectStreak()
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

        var statisticService = CreateStatisticService();

        // Act
        var streakData = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        streakData.LongestStreak.Should().Be(5);
        streakData.ReviewDates.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetStreakDataAsync_CurrentStreakActiveToday()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dates = new HashSet<DateOnly>
        {
            today.AddDays(-2),
            today.AddDays(-1),
            today
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(dates);
        
        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        result.LongestStreak.Should().Be(3);
        result.CurrentStreak.Should().Be(3);
    }

    [Fact]
    public async Task GetStreakDataAsync_CurrentStreakActiveYesterdayOnly()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dates = new HashSet<DateOnly>
        {
            today.AddDays(-3),
            today.AddDays(-2),
            today.AddDays(-1)  // Yesterday is the most recent review
        };
        
        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(dates);

        var statisticService = CreateStatisticService();
        
        // Act
        var result = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        result.LongestStreak.Should().Be(3);
        result.CurrentStreak.Should().Be(3);  // Still counts as current streak
    }

    [Fact]
    public async Task GetStreakDataAsync_StreakBrokenBeforeToday()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dates = new HashSet<DateOnly>
        {
            today.AddDays(-5),
            today.AddDays(-4),
            today.AddDays(-3),
            // Gap here - streak broken
            today.AddDays(-1)
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(dates);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        result.LongestStreak.Should().Be(3);
        result.CurrentStreak.Should().Be(1);
    }

    [Fact]
    public async Task GetStreakDataAsync_MultipleSeparateStreaks_ReturnLongestAndCurrent()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dates = new HashSet<DateOnly>
        {
            today.AddDays(-10),
            today.AddDays(-9),
            today.AddDays(-8), // First streak: 3 days
            // Gap
            today.AddDays(-5),
            today.AddDays(-4),
            today.AddDays(-3),
            today.AddDays(-2),
            today.AddDays(-1),
            today  // Second streak: 5 days (longest)
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(dates);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        result.LongestStreak.Should().Be(6);
        result.CurrentStreak.Should().Be(6);
    }

    [Fact]
    public async Task GetStreakDataAsync_ReturnsAllReviewDates()
    {
        // Arrange
        var dates = new HashSet<DateOnly>
        {
            new(2025, 1, 1),
            new(2025, 1, 2),
            new(2025, 1, 3)
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(dates);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        result.ReviewDates.Should().HaveCount(3);
        var expectedDates = new[] { new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 2), new DateOnly(2025, 1, 3) };
        result.ReviewDates.Should().ContainInOrder(expectedDates);
    }

    [Fact]
    public async Task GetStreakDataAsync_RepositoryCalledWithCorrectUserId()
    {
        // Arrange
        const string testUserId = "specific-user";
        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(testUserId))
            .ReturnsAsync([]);

        var statisticService = CreateStatisticService();

        // Act
        await statisticService.GetStreakDataAsync(testUserId);

        // Assert
        reviewRepo.Verify(r => r.GetDistinctReviewDatesAsync(testUserId), Times.Once);
    }

    [Fact]
    public async Task GetStreakDataAsync_NoReviewBetweenToday_AndYesterday_ResetStreak()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dates = new HashSet<DateOnly>
        {
            today.AddDays(-10),
            today.AddDays(-9)
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(dates);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        result.CurrentStreak.Should().Be(0);
        result.LongestStreak.Should().Be(2);
    }

    [Fact]
    public async Task GetStreakDataAsync_LargeDataset_PerformanceAcceptable()
    {
        // Arrange
        var dates = new HashSet<DateOnly>();
        for (var i = 0; i < 365; i++)
        {
            dates.Add(new DateOnly(2024, 1, 1).AddDays(i));
        }

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(dates);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        result.LongestStreak.Should().Be(365);
        result.ReviewDates.Should().HaveCount(365);
    }

    #endregion

    #region GetStatisticsAsync Tests

    [Fact]
    public async Task GetStatisticsAsync_ShouldCallAllRepositories()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var streakDates = new HashSet<DateOnly> { today };
        
        var xpData = new XpData
        {
            CurrentUserXp = 100,
            CurrentUserRank = 1,
            TopUsers = []
        };

        var dailyReviewData = new List<DailyReviewDto>
        {
            new() { Date = today, CardsReviewed = 5 }
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(streakDates);

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync(dailyReviewData);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert
        reviewRepo.Verify(r => r.GetDistinctReviewDatesAsync(defaultUserId), Times.Once);
        transactionRepo.Verify(t => t.GetXpStatistics(defaultUserId), Times.Once);
        reviewRepo.Verify(r => r.GetDailyReviewDataAsync(defaultUserId, 30), Times.Once);
    }

    [Fact]
    public async Task GetStatisticsAsync_ShouldReturnAggregatedData()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var streakDates = new HashSet<DateOnly> { today, today.AddDays(-1), today.AddDays(-2) };
        
        var xpData = new XpData
        {
            CurrentUserXp = 250,
            CurrentUserRank = 5,
            TopUsers = new List<LeaderboardEntryData>
            {
                new() { UserId = "user-2", Username = "player2", TotalXp = 500 }
            }
        };

        var dailyReviewData = new List<DailyReviewDto>
        {
            new() { Date = today, CardsReviewed = 10 },
            new() { Date = today.AddDays(-1), CardsReviewed = 8 },
            new() { Date = today.AddDays(-2), CardsReviewed = 12 }
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(streakDates);

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync(dailyReviewData);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert
        result.StreakData.Should().NotBeNull();
        result.StreakData.CurrentStreak.Should().Be(3);
        result.StreakData.LongestStreak.Should().Be(3);
        result.StreakData.ReviewDates.Should().HaveCount(3);

        result.XpData.Should().NotBeNull();
        result.XpData.CurrentUserXp.Should().Be(250);
        result.XpData.CurrentUserRank.Should().Be(5);
        result.XpData.TopUsers.Should().HaveCount(1);

        result.DailyReviewData.Should().HaveCount(3);
        result.DailyReviewData[0].CardsReviewed.Should().Be(10);
    }

    [Fact]
    public async Task GetStatisticsAsync_NoData_ReturnsEmptyCollections()
    {
        // Arrange
        var xpData = new XpData
        {
            CurrentUserXp = 0,
            CurrentUserRank = 0,
            TopUsers = []
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync([]);

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync([]);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert
        result.StreakData.CurrentStreak.Should().Be(0);
        result.StreakData.LongestStreak.Should().Be(0);
        result.XpData.TopUsers.Should().BeEmpty();
        result.DailyReviewData.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStatisticsAsync_MultipleTopLeaderboardEntries()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var streakDates = new HashSet<DateOnly> { today };

        var topUsers = new List<LeaderboardEntryData>
        {
            new() { UserId = "user-100", Username = "TopPlayer", TotalXp = 5000 },
            new() { UserId = "user-101", Username = "SecondPlace", TotalXp = 4500 },
            new() { UserId = "user-102", Username = "ThirdPlace", TotalXp = 4000 }
        };

        var xpData = new XpData
        {
            CurrentUserXp = 1000,
            CurrentUserRank = 50,
            TopUsers = topUsers
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(streakDates);

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync([]);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert
        result.XpData.TopUsers.Should().HaveCount(3);
        result.XpData.TopUsers[0].Username.Should().Be("TopPlayer");
        result.XpData.TopUsers[0].TotalXp.Should().Be(5000);
    }

    [Fact]
    public async Task GetStatisticsAsync_GetDailyReviewDataCalledWith30DayWindow()
    {
        // Arrange
        var streakDates = new HashSet<DateOnly>();
        var xpData = new XpData { CurrentUserXp = 0, CurrentUserRank = 0, TopUsers = [] };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(streakDates);

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync([]);

        var statisticService = CreateStatisticService();

        // Act
        await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert
        reviewRepo.Verify(
            r => r.GetDailyReviewDataAsync(defaultUserId, 30),
            Times.Once);
    }

    [Fact]
    public async Task GetStatisticsAsync_WithMultipleDailyReviewEntries()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var streakDates = new HashSet<DateOnly> { today };

        var dailyReviewData = new List<DailyReviewDto>();
        for (var i = 0; i < 30; i++)
        {
            dailyReviewData.Add(new DailyReviewDto
            {
                Date = today.AddDays(-i),
                CardsReviewed = (i % 20) + 1
            });
        }

        var xpData = new XpData { CurrentUserXp = 0, CurrentUserRank = 0, TopUsers = [] };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(streakDates);

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync(dailyReviewData);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert
        result.DailyReviewData.Should().HaveCount(30);
        result.DailyReviewData.Should().AllSatisfy(d => d.CardsReviewed.Should().BeGreaterThan(0));
    }

    [Fact]
    public async Task GetStatisticsAsync_ReturnsNotNullStatisticDto()
    {
        // Arrange
        var streakDates = new HashSet<DateOnly>();
        var xpData = new XpData { CurrentUserXp = 0, CurrentUserRank = 0, TopUsers = [] };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(streakDates);

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync([]);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert
        result.Should().NotBeNull();
        result.StreakData.Should().NotBeNull();
        result.XpData.Should().NotBeNull();
        result.DailyReviewData.Should().NotBeNull();
    }

    #endregion

    #region Edge Cases and Special Scenarios

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(30)]
    [InlineData(100)]
    public async Task GetStreakDataAsync_VariousStreakLengths(int streakLength)
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dates = new HashSet<DateOnly>();

        for (var i = 0; i < streakLength; i++)
        {
            dates.Add(today.AddDays(-i));
        }

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(dates);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        result.CurrentStreak.Should().Be(streakLength);
        result.LongestStreak.Should().Be(streakLength);
    }

    [Fact]
    public async Task GetStreakDataAsync_ReviewDatesContainAllInputDates()
    {
        // Arrange
        var dates = new HashSet<DateOnly>
        {
            new(2025, 1, 1),
            new(2025, 1, 5),
            new(2025, 1, 10),
            new(2025, 2, 1)
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(dates);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        foreach (var date in dates)
        {
            result.ReviewDates.Should().Contain(date);
        }
    }

    [Fact]
    public async Task GetStreakDataAsync_ShouldReturnCorrectStreakDataType()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(new HashSet<DateOnly> { today });

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<StreakData>();
        result.CurrentStreak.Should().BeGreaterThanOrEqualTo(0);
        result.LongestStreak.Should().BeGreaterThanOrEqualTo(0);
        result.ReviewDates.Should().NotBeNull();
    }

    [Fact]
    public async Task GetStreakDataAsync_CurrentStreakCalculatedCorrectly_WhenMultipleBreaks()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dates = new HashSet<DateOnly>
        {
            today.AddDays(-20),  // Old streak
            today.AddDays(-19),
            today.AddDays(-18),
            // Gap
            today.AddDays(-5),   // Medium streak
            today.AddDays(-4),
            today.AddDays(-3),
            // Gap
            today.AddDays(-1),   // Current/recent streak (most recent)
            today
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(dates);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        result.CurrentStreak.Should().Be(2);  // Only the most recent (yesterday + today)
        result.LongestStreak.Should().Be(3);  // The longest period
    }

    [Fact]
    public async Task GetStreakDataAsync_LongestStreakIsMaximumFound()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dates = new HashSet<DateOnly>
        {
            today.AddDays(-15),
            today.AddDays(-14),
            // Gap
            today.AddDays(-10),
            today.AddDays(-9),
            today.AddDays(-8),
            today.AddDays(-7),
            today.AddDays(-6)  // This streak is 5 days (longest)
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(dates);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStreakDataAsync(defaultUserId);

        // Assert
        result.LongestStreak.Should().Be(5);
    }

    #endregion

    #region GetStatisticsAsync Extended Tests

    [Fact]
    public async Task GetStatisticsAsync_ShouldThrow_WhenUserIdIsNull()
    {
        // Arrange
        var statisticService = CreateStatisticService();

        // Act
        var act = () => statisticService.GetStatisticsAsync(null!);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetStatisticsAsync_WithDifferentUserIds()
    {
        // Arrange
        var testUserId = "specific-user-id-123";
        var streakDates = new HashSet<DateOnly>();
        var xpData = new XpData { CurrentUserXp = 42, CurrentUserRank = 7, TopUsers = [] };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(testUserId))
            .ReturnsAsync(streakDates);

        transactionRepo.Setup(t => t.GetXpStatistics(testUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(testUserId, 30))
            .ReturnsAsync([]);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(testUserId);

        // Assert
        result.XpData.CurrentUserXp.Should().Be(42);
        result.XpData.CurrentUserRank.Should().Be(7);
        reviewRepo.Verify(r => r.GetDistinctReviewDatesAsync(testUserId), Times.Once);
        transactionRepo.Verify(t => t.GetXpStatistics(testUserId), Times.Once);
        reviewRepo.Verify(r => r.GetDailyReviewDataAsync(testUserId, 30), Times.Once);
    }

    [Fact]
    public async Task GetStatisticsAsync_IntegrationWithActiveStreak()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var streakDates = new HashSet<DateOnly>
        {
            today.AddDays(-6),
            today.AddDays(-5),
            today.AddDays(-4),
            today.AddDays(-3),
            today.AddDays(-2),
            today.AddDays(-1),
            today  // 7-day active streak
        };

        var xpData = new XpData
        {
            CurrentUserXp = 1500,
            CurrentUserRank = 15,
            TopUsers = new List<LeaderboardEntryData>
            {
                new() { UserId = "top-1", Username = "ChampionPlayer", TotalXp = 10000 }
            }
        };

        var dailyReviewData = new List<DailyReviewDto>
        {
            new() { Date = today, CardsReviewed = 50 },
            new() { Date = today.AddDays(-1), CardsReviewed = 45 },
            new() { Date = today.AddDays(-2), CardsReviewed = 40 }
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(streakDates);

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync(dailyReviewData);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert
        result.StreakData.CurrentStreak.Should().Be(7);
        result.StreakData.LongestStreak.Should().Be(7);
        result.XpData.CurrentUserXp.Should().Be(1500);
        result.DailyReviewData[0].CardsReviewed.Should().Be(50);
    }

    [Fact]
    public async Task GetStatisticsAsync_ConcurrentDataCollection()
    {
        // Arrange
        var streakDates = new HashSet<DateOnly>();
        var xpData = new XpData { CurrentUserXp = 100, CurrentUserRank = 20, TopUsers = [] };
        var dailyData = new List<DailyReviewDto>();

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(streakDates);

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync(dailyData);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert - Verify all three calls were made
        reviewRepo.Verify(r => r.GetDistinctReviewDatesAsync(defaultUserId), Times.Once);
        transactionRepo.Verify(t => t.GetXpStatistics(defaultUserId), Times.Once);
        reviewRepo.Verify(r => r.GetDailyReviewDataAsync(defaultUserId, 30), Times.Once);

        // Verify data properly aggregated
        result.StreakData.Should().NotBeNull();
        result.XpData.Should().NotBeNull();
        result.DailyReviewData.Should().NotBeNull();
    }

    [Fact]
    public async Task GetStatisticsAsync_MissingLeaderboardData()
    {
        // Arrange
        var xpData = new XpData
        {
            CurrentUserXp = 500,
            CurrentUserRank = 100,
            TopUsers = []  // Empty leaderboard
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync([]);

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync([]);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert
        result.Should().NotBeNull();
        result.XpData.TopUsers.Should().BeEmpty();
        result.XpData.CurrentUserXp.Should().Be(500);
        result.XpData.CurrentUserRank.Should().Be(100);
    }

    [Fact]
    public async Task GetStatisticsAsync_LargeLeaderboardData()
    {
        // Arrange
        var topUsers = new List<LeaderboardEntryData>();
        for (var i = 0; i < 100; i++)
        {
            topUsers.Add(new LeaderboardEntryData
            {
                UserId = $"user-{i}",
                Username = $"Player{i}",
                TotalXp = 10000 - (i * 50)
            });
        }

        var xpData = new XpData
        {
            CurrentUserXp = 1000,
            CurrentUserRank = 200,
            TopUsers = topUsers
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync([]);

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync([]);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert
        result.XpData.TopUsers.Should().HaveCount(100);
        result.XpData.TopUsers[0].Username.Should().Be("Player0");
        result.XpData.TopUsers[99].Username.Should().Be("Player99");
    }

    [Fact]
    public async Task GetStatisticsAsync_BoundaryValuesInXpData()
    {
        // Arrange
        var xpData = new XpData
        {
            CurrentUserXp = int.MaxValue,
            CurrentUserRank = 1,
            TopUsers = []
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync([]);

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync([]);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert
        result.XpData.CurrentUserXp.Should().Be(int.MaxValue);
        result.XpData.CurrentUserRank.Should().Be(1);
    }

    [Fact]
    public async Task GetStatisticsAsync_FullMonthOfDailyReviewData()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var streakDates = new HashSet<DateOnly> { today };

        var dailyReviewData = new List<DailyReviewDto>();
        var totalCards = 0;
        for (var i = 29; i >= 0; i--)
        {
            var cardsToday = 5 + (i % 10);  // Vary between 5-14 cards
            totalCards += cardsToday;
            dailyReviewData.Add(new DailyReviewDto
            {
                Date = today.AddDays(-i),
                CardsReviewed = cardsToday
            });
        }

        var xpData = new XpData { CurrentUserXp = 0, CurrentUserRank = 0, TopUsers = [] };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(streakDates);

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync(dailyReviewData);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert
        result.DailyReviewData.Should().HaveCount(30);
        result.DailyReviewData.Sum(d => d.CardsReviewed).Should().Be(totalCards);
        result.DailyReviewData.Should().AllSatisfy(d => d.CardsReviewed.Should().BeGreaterThan(0));
    }

    [Fact]
    public async Task GetStatisticsAsync_UserWithZeroXp()
    {
        // Arrange
        var xpData = new XpData
        {
            CurrentUserXp = 0,
            CurrentUserRank = 0,
            TopUsers = []
        };

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync([]);

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync([]);

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert
        result.XpData.CurrentUserXp.Should().Be(0);
        result.XpData.CurrentUserRank.Should().Be(0);
        result.DailyReviewData.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStatisticsAsync_ReturnStructureConsistency()
    {
        // Arrange
        var xpData = new XpData
        {
            CurrentUserXp = 500,
            CurrentUserRank = 50,
            TopUsers = new List<LeaderboardEntryData>
            {
                new() { UserId = "1", Username = "User1", TotalXp = 5000 }
            }
        };

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        reviewRepo.Setup(r => r.GetDistinctReviewDatesAsync(defaultUserId))
            .ReturnsAsync(new HashSet<DateOnly> { today });

        transactionRepo.Setup(t => t.GetXpStatistics(defaultUserId))
            .ReturnsAsync(xpData);

        reviewRepo.Setup(r => r.GetDailyReviewDataAsync(defaultUserId, 30))
            .ReturnsAsync(new List<DailyReviewDto>
            {
                new() { Date = today, CardsReviewed = 10 }
            });

        var statisticService = CreateStatisticService();

        // Act
        var result = await statisticService.GetStatisticsAsync(defaultUserId);

        // Assert - Verify structure
        result.Should().BeOfType<StatisticDto>();
        result.StreakData.Should().BeOfType<StreakData>();
        result.XpData.Should().BeOfType<XpData>();
        result.DailyReviewData.Should().BeOfType<List<DailyReviewDto>>();
        
        // Verify no nulls
        result.StreakData.ReviewDates.Should().NotBeNull();
        result.XpData.TopUsers.Should().NotBeNull();
    }

    #endregion
}