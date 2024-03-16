using BookApi.Domain.Interfaces;

namespace BookApi.Domain.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
