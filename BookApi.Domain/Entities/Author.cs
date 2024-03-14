using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Entities;

public class Author : AggregateEntityBase<Author>
{
    public AuthorName Name { get; private set; }
    public ItemID[] Books { get; } = [];

    public Author(
        int id,
        DateTime createdAt, DateTime? updatedAt,
        int createdByID, string createdByName,
        int? updatedByID, string? updatedByName,
        int versionID,
        string name,
        int[] bookIDs
    ) : base(id, createdAt, updatedAt, createdByID, createdByName, updatedByID, updatedByName, versionID)
    {
        Name = AuthorName.Reconstruct(name);
        Books = [.. bookIDs.Select(ItemID.Reconstruct)];
    }

    private Author(string name)
    {
        Name = AuthorName.CreateWithValidation(name);
    }

    public static Author CreateNew(IActorPermission permission, string name)
        => new Author(name).CreateNew(permission);

    public void Update(IActorPermission permission, string name)
    {
        Name = AuthorName.CreateWithValidation(name);
        Update(permission);
    }
}
