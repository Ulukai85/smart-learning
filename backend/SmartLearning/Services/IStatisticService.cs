using SmartLearning.DTOs;

namespace SmartLearning.Services;

public interface IStatisticService
{
    Task<StatisticDto> GetStatisticsAsync(string userId);
    Task<StreakData> GetStreakDataAsync(string userId);
}