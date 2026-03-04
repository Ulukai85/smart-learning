namespace SmartLearning.DTOs;

public class StatisticDto
{
    public StreakData StreakData { get; set; }
    public XpData XpData { get; set; }
    public List<DailyReviewDto> DailyReviewData { get; set; }
}

public class StreakData 
{
    public int LongestStreak { get; set; }
    public int CurrentStreak { get; set; }
    public List<DateOnly> ReviewDates { get; set; }
}

public class XpData
{
    public int CurrentUserXP { get; set; }
    public int CurrentUserRank { get; set; }
    public List<LeaderboardEntryData> TopUsers { get; set; }
}

public class LeaderboardEntryData
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public int TotalXp { get; set; }
}

public class DailyReviewDto
{
    public DateOnly Date { get; set; }
    public int CardsReviewed { get; set; }
}
