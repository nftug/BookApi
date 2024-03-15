namespace BookApi.Domain.ValueObjects.Shared;

public record DateTimeRecord
{
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
