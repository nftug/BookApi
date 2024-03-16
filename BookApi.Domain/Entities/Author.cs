using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Entities;

public class Author : AggregateEntityBase<Author>
{
    public AuthorName Name { get; private set; }
    public ItemId[] Books { get; } = [];

    public Author(
        int id,
        DateTime createdAt, DateTime? updatedAt,
        int createdById, string createdByName,
        int? updatedById, string? updatedByName,
        int versionId,
        string name,
        int[] bookIds
    ) : base(id, createdAt, updatedAt, createdById, createdByName, updatedById, updatedByName, versionId)
    {
        Name = AuthorName.Reconstruct(name);
        Books = [.. bookIds.Select(ItemId.Reconstruct)];
    }

    private Author(string name)
    {
        Name = AuthorName.CreateWithValidation(name);
    }

    internal static Author CreateNew(AdminOnlyPermission permission, string name)
        => new Author(name).CreateNew(permission);

    internal void Update(AdminOnlyPermission permission, string name)
    {
        Name = AuthorName.CreateWithValidation(name);
        Update(permission);
    }
}
