using BookApi.Domain.Abstractions.ValueObjects;

namespace BookApi.Domain.ValueObjects.Shared;

public record Actor(ItemId Id, string UserId, bool IsAdmin) : IActor
{
    public static Actor CreateNewActor(string userId) => new(ItemId.Reconstruct(0), userId, false);
}

public record ActorForRecord(string UserId) : IActor;