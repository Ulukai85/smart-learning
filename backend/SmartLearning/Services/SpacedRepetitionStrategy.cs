namespace SmartLearning.Services;

public interface ISpacedRepetitionStrategy
{
    bool ShouldReinsert(int grade, string strategyDataJson);
    DateTime CalculateNextReview(int grade, DateTime now, string strategyDataJson);
}

public class AnkiSpacedRepetition : ISpacedRepetitionStrategy
{
    public bool ShouldReinsert(int grade, string strategyDataJson)
    {
        return grade == 0;
    }
    
    public DateTime CalculateNextReview(int grade, DateTime now, string strategyDataJson)
    {
        return grade switch
        {
            0 => now,
            1 => now.AddDays(2),
            _ => now.AddDays(4)
        };
    }
}