using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.ValueObjects.Users;

namespace BookApi.Domain.ValueObjects.Shared;

public record Actor(ItemId Id, UserId UserId, bool IsAdmin) : IActor
{
    public static Actor CreateNewActor(UserId userId) => new(ItemId.Reconstruct(0), userId, false);
}

public record ActorForRecord(UserId UserId) : IActor;