using BookApi.Domain.Abstractions.ValueObjects;

namespace BookApi.Domain.ValueObjects.Shared;

public record ActorForRecord(string UserId) : IActor;

public record ActorForPermission(ItemId Id, string UserId, bool IsAdmin) : IActor;
