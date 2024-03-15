using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Entities;

public class Publisher : AggregateEntityBase<Publisher>
{
    public PublisherName Name { get; private set; }
    public ItemID[] Books { get; } = [];

    public Publisher(
        int id,
        DateTime createdAt, DateTime? updatedAt,
        int createdByID, string createdByName,
        int? updatedByID, string? updatedByName,
        int versionID,
        string name,
        int[] bookIDs
    ) : base(id, createdAt, updatedAt, createdByID, createdByName, updatedByID, updatedByName, versionID)
    {
        Name = PublisherName.Reconstruct(name);
        Books = [.. bookIDs.Select(ItemID.Reconstruct)];
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
