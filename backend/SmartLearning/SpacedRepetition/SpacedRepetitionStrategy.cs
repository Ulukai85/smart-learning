namespace SmartLearning.Services;

public interface ISpacedRepetitionStrategy
{
    bool ShouldReinsert(int grade, string strategyDataJson);
    DateTime CalculateNextReview(int grade, DateTime now, string strategyDataJson);
}

public class AnkiSpacedRepetition : ISpacedRepetitionStrategy
{
    // 0: Again, 1: Hard, 2: Good, 3: Easy
    public bool ShouldReinsert(int grade, string strategyDataJson)
    {
        return grade == 0;
    }
    
    public DateTime CalculateNextReview(int grade, DateTime now, string strategyDataJson)
    {
        return grade switch
        {
            0 => now,
            1 => now.AddDays(1),
            2 => now.AddDays(4),
            _ => now.AddDays(8)
        };
    }
}