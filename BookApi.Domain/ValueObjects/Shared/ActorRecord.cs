using BookApi.Domain.Abstractions.ValueObjects;

namespace BookApi.Domain.ValueObjects.Shared;

public record ActorRecord
{
    public required IActor CreatedBy { get; init; }
    public IActor? UpdatedBy { get; init; }

    public static ActorRecord Reconstruct(
        int createdById, string createdByName,
        int? updatedById, string? updatedByName
    )
    {
        return new()
        {
            CreatedBy = new ActorForRecord(createdById, createdByName),
            UpdatedBy =
                updatedById is { } _updatedById && updatedByName is { }
                ? new ActorForRecord(_updatedById, updatedByName)
                : null
        };
    }
}
