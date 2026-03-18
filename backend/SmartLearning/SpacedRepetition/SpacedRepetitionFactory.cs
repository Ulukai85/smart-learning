namespace SmartLearning.SpacedRepetition;

public class SpacedRepetitionFactory : ISpacedRepetitionFactory
{
    public ISpacedRepetitionStrategy GetStrategy(string strategyType)
    {
        return strategyType switch
        {
            "Anki" => new AnkiStrategy(),
            "AnkiV2" => new AnkiV2Strategy(),
            _ => throw new ArgumentException($"Unknown strategy type: {strategyType}")
        };
    }
}