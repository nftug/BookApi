using System.Collections;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.ValueObjects.Books;

public record BookLike(ItemId LikedByItemId, DateTime LikedAt)
{
    public static BookLike Reconstruct(int likedByItemId, DateTime likedAt)
        => new(ItemId.Reconstruct(likedByItemId), likedAt);
}

public record BookLikeList : IEnumerable<BookLike>
{
    private readonly IEnumerable<BookLike> _items;

    private BookLikeList(IEnumerable<BookLike> items) => _items = [.. items];

    public static BookLikeList Reconstruct(IEnumerable<BookLike> bookLikes)
        => new(bookLikes.Distinct().OrderBy(x => x.LikedAt));

    public static BookLikeList Empty() => new(Enumerable.Empty<BookLike>());

    internal BookLikeList RecreateWithToggle(PassThroughPermission permission, IDateTimeProvider dateTimeProvider)
    {
        bool haveLikedByActor = _items.Any(x => x.LikedByItemId == permission.Actor.Id);
        return haveLikedByActor
            ? ItemsWithoutUser(permission.Actor.Id)
            : ItemsWithUser(permission.Actor.Id, dateTimeProvider);
    }

    internal BookLikeList RecreateWithEdit(
        AdminOnlyPermission permission,
        IDateTimeProvider dateTimeProvider,
        ItemId userItemId, bool doLikeBook
    )
    {
        if (!permission.CanUpdate) throw new ForbiddenException();
        return doLikeBook
            ? ItemsWithoutUser(userItemId)
            : ItemsWithUser(userItemId, dateTimeProvider);
    }

    private BookLikeList ItemsWithoutUser(ItemId userItemId)
        => Reconstruct(_items.Where(x => x.LikedByItemId != userItemId));

    private BookLikeList ItemsWithUser(ItemId userItemId, IDateTimeProvider dateTimeProvider)
    {
        var newLike = new BookLike(userItemId, dateTimeProvider.UtcNow);
        return Reconstruct(_items.Append(newLike));
    }

    public IEnumerator<BookLike> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
}