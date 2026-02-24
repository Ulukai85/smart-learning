namespace SmartLearning.Services;

public interface ISpacedRepetition
{
    bool ShouldReinsert(int grade);
    DateTime CalculateNextReview(int grade, DateTime now);
}

public class SpacedRepetition : ISpacedRepetition
{
    public bool ShouldReinsert(int grade)
    {
        return grade == 0;
    }
    
    public DateTime CalculateNextReview(int grade, DateTime now)
    {
        return grade switch
        {
            0 => now,
            1 => now.AddDays(2),
            _ => now.AddDays(4)
        };
    }
}