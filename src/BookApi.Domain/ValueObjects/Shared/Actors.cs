using BookApi.Domain.Abstractions.ValueObjects;

namespace BookApi.Domain.ValueObjects.Shared;

public record Actor(ItemId Id, string UserId, bool IsAdmin) : IActor;

public record ActorForRecord(string UserId) : IActor;