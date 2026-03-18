using System.Text.Json;

namespace SmartLearning.SpacedRepetition;

public interface ISpacedRepetitionStrategy
{
    bool ShouldReinsert(int grade, string strategyDataJson);
    DateTime CalculateNextReview(int grade, DateTime now, string strategyDataJson);
    string UpdateStrategyData(int grade, string strategyDataJson);
}

public class AnkiStrategyData
{
    public int Interval { get; set; }
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

    public string UpdateStrategyData(int grade, string strategyDataJson)
    {
        return "{}";
    }
}

public class AnkiV2 : ISpacedRepetitionStrategy
{
    // 0: Again, 1: Hard, 2: Good, 3: Easy
    public bool ShouldReinsert(int grade, string strategyDataJson)
    {
        return grade == 0;
    }
    
    public DateTime CalculateNextReview(int grade, DateTime now, string strategyDataJson)
    {
        var strategyData = JsonSerializer.Deserialize<AnkiStrategyData>(strategyDataJson);
        var interval = strategyData?.Interval ?? 1;
        
        return grade switch
        {
            0 => now,
            1 => now.AddDays(1 * interval),
            2 => now.AddDays(4 * interval),
            _ => now.AddDays(8 * interval)
        };
    }
    
    public string UpdateStrategyData(int grade, string strategyDataJson)
    {
        var strategyData = JsonSerializer.Deserialize<AnkiStrategyData>(strategyDataJson)
                           ?? new AnkiStrategyData { Interval = 0 };

        if (grade == 0)
        {
            strategyData.Interval = 1;
        }
        else
        {
            strategyData.Interval += 1;
        }
        
        return JsonSerializer.Serialize(strategyData);
    }
}