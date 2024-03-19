using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.Interfaces;
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
        string createdByUserId, string? updatedByUserId,
        int versionId,
        string name,
        int[] bookIds
    ) : base(id, createdAt, updatedAt, createdByUserId, updatedByUserId, versionId)
    {
        Name = AuthorName.Reconstruct(name);
        Books = [.. bookIds.Select(ItemId.Reconstruct)];
    }

    private Author(string name)
    {
        Name = AuthorName.CreateWithValidation(name);
    }

    internal static Author CreateNew(
        AdminOnlyPermission permission, IDateTimeProvider dateTimeProvider, string name
    )
        => new Author(name).CreateNew(permission, dateTimeProvider);

    internal void Update(
        AdminOnlyPermission permission, IDateTimeProvider dateTimeProvider, string name
    )
    {
        Name = AuthorName.CreateWithValidation(name);
        Update(permission, dateTimeProvider);
    }
}
