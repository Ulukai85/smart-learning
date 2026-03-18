namespace SmartLearning.SpacedRepetition;

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
            "AnkiV2" => new AnkiV2(),
            _ => throw new ArgumentException($"Unknown strategy type: {strategyType}")
        };
    }
}