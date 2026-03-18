namespace SmartLearning.Utils;

public interface ITimeProvider
{
    DateTime UtcNow { get; }
}