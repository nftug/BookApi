using BookApi.Domain.Abstractions.ValueObjects;

namespace BookApi.Domain.ValueObjects.Shared;

public record ActorRecord
{
    public required IActor CreatedBy { get; init; }
    public IActor? UpdatedBy { get; init; }

    public static ActorRecord Reconstruct(
        int createdByID, string createdByName,
        int? updatedByID, string? updatedByName
    )
    {
        return new()
        {
            CreatedBy = new ActorForRecord(createdByID, createdByName),
            UpdatedBy =
                updatedByID is { } _updatedByID && updatedByName is { }
                ? new ActorForRecord(_updatedByID, updatedByName)
                : null
        };
    }
}
