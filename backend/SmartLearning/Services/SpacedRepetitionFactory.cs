namespace SmartLearning.Services;

public interface ISpacedRepetitionFactory
{
    ISpacedRepetitionStrategy GetStrategy(string strategyType);
}

public class SpacedRepetitionFactory : ISpacedRepetitionFactory
{
    public ISpacedRepetitionStrategy GetStrategy(string strategyType)
    {
        return strategyType switch
        {
            "Anki" => new AnkiSpacedRepetition(),
            _ => throw new ArgumentException($"Unknown strategy type: {strategyType}")
        };
    }
}