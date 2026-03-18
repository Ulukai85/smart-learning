using SmartLearning.Services;

namespace SmartLearning.Utils;

public class SystemTimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}