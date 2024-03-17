using BookApi.Domain.Abstractions.ValueObjects;

namespace BookApi.Domain.ValueObjects.Shared;

public record ActorForRecord(int UserId, string UserName) : IActor;

public record ActorForPermission(int UserId, string UserName, bool IsAdmin) : IActor;
