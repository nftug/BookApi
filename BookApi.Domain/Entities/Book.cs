using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Entities;

public class Book : AggregateEntityBase<Book>
{
    public BookTitle Title { get; private set; }
    public ISBNCode ISBN { get; private set; }
    public BookAuthorList Authors { get; private set; }
    public ItemId Publisher { get; private set; }
    public BookPublicationDate PublishedAt { get; private set; }

    public Book(
        int id,
        DateTime createdAt, DateTime? updatedAt,
        int createdById, string createdByName,
        int? updatedById, string? updatedByName,
        int versionId,
        string title,
        string isbn,
        int[] authorIds,
        int publisherId,
        DateTime publishedAt
    ) : base(id, createdAt, updatedAt, createdById, createdByName, updatedById, updatedByName, versionId)
    {
        Title = BookTitle.Reconstruct(title);
        ISBN = ISBNCode.Reconstruct(isbn);
        Authors = BookAuthorList.Reconstruct(authorIds);
        Publisher = ItemId.Reconstruct(publisherId);
        PublishedAt = BookPublicationDate.Reconstruct(publishedAt);
    }

    private Book(string title, string isbn, int[] authorIds, int publisherId, DateTime publishedAt)
    {
        Title = BookTitle.CreateWithValidation(title);
        ISBN = ISBNCode.CreateWithValidation(isbn);
        Authors = BookAuthorList.CreateWithValidation(authorIds);
        Publisher = ItemId.CreateWithValidation(publisherId);
        PublishedAt = BookPublicationDate.CreateWithValidation(publishedAt);
    }

    internal static Book CreateNew(
        AdminOnlyPermission permission,
        IDateTimeProvider dateTimeProvider,
        string name, string isbn, int[] authorIds, int publisherId, DateTime publishedAt
    )
        => new Book(name, isbn, authorIds, publisherId, publishedAt)
            .CreateNew(permission, dateTimeProvider);

    internal void Update(
        AdminOnlyPermission permission,
        IDateTimeProvider dateTimeProvider,
        string title, string isbn, int[] authorIds, int publisherId, DateTime publishedAt
    )
    {
        Title = BookTitle.CreateWithValidation(title);
        ISBN = ISBNCode.CreateWithValidation(isbn);
        Authors = BookAuthorList.CreateWithValidation(authorIds);
        Publisher = ItemId.CreateWithValidation(publisherId);
        PublishedAt = BookPublicationDate.CreateWithValidation(publishedAt);
        Update(permission, dateTimeProvider);
    }
}
