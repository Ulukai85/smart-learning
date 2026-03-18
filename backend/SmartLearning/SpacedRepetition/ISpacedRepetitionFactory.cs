namespace SmartLearning.SpacedRepetition;

public interface ISpacedRepetitionFactory
{
    ISpacedRepetitionStrategy GetStrategy(string strategyType);
}