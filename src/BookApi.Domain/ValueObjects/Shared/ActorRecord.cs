using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.ValueObjects.Users;

namespace BookApi.Domain.ValueObjects.Shared;

public record ActorRecord
{
    public required IActor CreatedBy { get; init; }
    public IActor? UpdatedBy { get; init; }

    public static ActorRecord Reconstruct(UserId createdByUserId, UserId? updatedByUserId)
    {
        return new()
        {
            CreatedBy = new ActorForRecord(createdByUserId),
            UpdatedBy =
                updatedByUserId is { } ? new ActorForRecord(updatedByUserId) : null
        };
    }
}
