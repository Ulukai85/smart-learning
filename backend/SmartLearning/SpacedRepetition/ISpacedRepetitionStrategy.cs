using System.Text.Json;

namespace SmartLearning.SpacedRepetition;

public interface ISpacedRepetitionStrategy
{
    bool ShouldReinsert(int grade, string strategyDataJson);
    DateTime CalculateNextReview(int grade, DateTime now, string strategyDataJson);
    string UpdateStrategyData(int grade, string strategyDataJson);
}