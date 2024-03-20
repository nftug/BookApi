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
    public BookLikeList Likes { get; private set; }

    public int CountOfLikes => Likes.Count();

    public Book(
        int id,
        DateTime createdAt, DateTime? updatedAt,
        string createdByUserId, string? updatedByUserId,
        int versionId,
        string title,
        string isbn,
        int[] authorIds,
        int publisherId,
        DateTime publishedAt,
        IEnumerable<BookLike> bookLikes
    ) : base(id, createdAt, updatedAt, createdByUserId, updatedByUserId, versionId)
    {
        Title = BookTitle.Reconstruct(title);
        ISBN = ISBNCode.Reconstruct(isbn);
        Authors = BookAuthorList.Reconstruct(authorIds);
        Publisher = ItemId.Reconstruct(publisherId);
        PublishedAt = BookPublicationDate.Reconstruct(publishedAt);
        Likes = BookLikeList.Reconstruct(bookLikes);
    }

    private Book(string title, string isbn, int[] authorIds, int publisherId, DateTime publishedAt)
    {
        Title = BookTitle.CreateWithValidation(title);
        ISBN = ISBNCode.CreateWithValidation(isbn);
        Authors = BookAuthorList.CreateWithValidation(authorIds);
        Publisher = ItemId.CreateWithValidation(publisherId);
        PublishedAt = BookPublicationDate.CreateWithValidation(publishedAt);
        Likes = BookLikeList.Empty();
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

    internal void ToggleLike(PassThroughPermission permission, IDateTimeProvider dateTimeProvider)
    {
        Likes = Likes.RecreateWithToggle(permission, dateTimeProvider);
    }

    internal void EditLike(
        AdminOnlyPermission permission, IDateTimeProvider dateTimeProvider,
        ItemId userItemId, bool doLikeBook
    )
    {
        Likes = Likes.RecreateWithEdit(permission, dateTimeProvider, userItemId, doLikeBook);
    }

    public bool IsLikedBy(ItemId userItemId) => Likes.IsLikedBy(userItemId);

    public bool IsLikedByMe(Actor actor) => Likes.IsLikedBy(actor.Id);
}
