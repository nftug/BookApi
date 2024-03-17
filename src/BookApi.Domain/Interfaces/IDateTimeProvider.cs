namespace BookApi.Domain.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateTime LocalTimeNow => UtcNow.ToLocalTime();
}
