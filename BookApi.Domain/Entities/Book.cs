using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Entities;

public class Book : AggregateEntityBase<Book>
{
    public BookTitle Title { get; private set; }
    public ISBNCode ISBN { get; private set; }
    public BookAuthorList Authors { get; private set; }
    public ItemID Publisher { get; private set; }
    public BookPublicationDate PublishedAt { get; private set; }

    public Book(
        int id,
        DateTime createdAt, DateTime? updatedAt,
        int createdByID, string createdByName,
        int? updatedByID, string? updatedByName,
        int versionID,
        string title,
        string isbn,
        int[] authorIDs,
        int publisherID,
        DateTime publishedAt
    ) : base(id, createdAt, updatedAt, createdByID, createdByName, updatedByID, updatedByName, versionID)
    {
        Title = BookTitle.Reconstruct(title);
        ISBN = ISBNCode.Reconstruct(isbn);
        Authors = BookAuthorList.Reconstruct(authorIDs);
        Publisher = ItemID.Reconstruct(publisherID);
        PublishedAt = BookPublicationDate.Reconstruct(publishedAt);
    }

    private Book(string title, string isbn, int[] authorIDs, int publisherID, DateTime publishedAt)
    {
        Title = BookTitle.CreateWithValidation(title);
        ISBN = ISBNCode.CreateWithValidation(isbn);
        Authors = BookAuthorList.CreateWithValidation(authorIDs);
        Publisher = ItemID.CreateWithValidation(publisherID);
        PublishedAt = BookPublicationDate.CreateWithValidation(publishedAt);
    }

    internal static Book CreateNew(
        AdminOnlyPermission permission,
        string name, string isbn, int[] authorIDs, int publisherID, DateTime publishedAt
    )
        => new Book(name, isbn, authorIDs, publisherID, publishedAt).CreateNew(permission);

    internal void Update(
        AdminOnlyPermission permission,
        string title, string isbn, int[] authorIDs, int publisherID, DateTime publishedAt
    )
    {
        Title = BookTitle.CreateWithValidation(title);
        ISBN = ISBNCode.CreateWithValidation(isbn);
        Authors = BookAuthorList.CreateWithValidation(authorIDs);
        Publisher = ItemID.CreateWithValidation(publisherID);
        PublishedAt = BookPublicationDate.CreateWithValidation(publishedAt);
        Update(permission);
    }
}
