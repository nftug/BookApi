using BookApi.Domain.Abstractions.Entities;
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
        int createdById, string createdByName,
        int? updatedById, string? updatedByName,
        int versionId,
        string name,
        int[] bookIds
    ) : base(id, createdAt, updatedAt, createdById, createdByName, updatedById, updatedByName, versionId)
    {
        Name = PublisherName.Reconstruct(name);
        Books = [.. bookIds.Select(ItemId.Reconstruct)];
    }

    private Publisher(string name)
    {
        Name = PublisherName.CreateWithValidation(name);
    }

    internal static Publisher CreateNew(AdminOnlyPermission permission, string name)
        => new Publisher(name).CreateNew(permission);

    internal void Update(AdminOnlyPermission permission, string name)
    {
        Name = PublisherName.CreateWithValidation(name);
        Update(permission);
    }
}
