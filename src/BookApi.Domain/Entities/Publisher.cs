using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Entities;

public class Publisher : AggregateEntityBase<Publisher>
{
    public PublisherName Name { get; private set; }
    public ItemId[] Books { get; } = [];

    public Publisher(
        int id,
        DateTime createdAt, DateTime? updatedAt,
        string createdByUserId, string? updatedByUserId,
        int versionId,
        string name,
        int[] bookIds
    ) : base(id, createdAt, updatedAt, createdByUserId, updatedByUserId, versionId)
    {
        Name = PublisherName.Reconstruct(name);
        Books = [.. bookIds.Select(ItemId.Reconstruct)];
    }

    private Publisher(string name)
    {
        Name = PublisherName.CreateWithValidation(name);
    }

    internal static Publisher CreateNew(
        AdminOnlyPermission permission, IDateTimeProvider dateTimeProvider, string name
    )
        => new Publisher(name).CreateNew(permission, dateTimeProvider);

    internal void Update(
        AdminOnlyPermission permission, IDateTimeProvider dateTimeProvider, string name
        )
    {
        Name = PublisherName.CreateWithValidation(name);
        Update(permission, dateTimeProvider);
    }
}
