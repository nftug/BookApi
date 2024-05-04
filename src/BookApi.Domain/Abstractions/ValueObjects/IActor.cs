using BookApi.Domain.ValueObjects.Users;

namespace BookApi.Domain.Abstractions.ValueObjects;

public interface IActor
{
    public UserId UserId { get; }
}
