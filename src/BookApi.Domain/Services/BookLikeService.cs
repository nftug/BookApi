using BookApi.Domain.Entities;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Services;

public class BookLikeService(IBookRepository bookRepository, IDateTimeProvider dateTimeProvider)
{
    public async Task ToggleLikeBookAsync(PassThroughPermission permission, Book book)
    {
        book.ToggleLike(permission, dateTimeProvider);
        await bookRepository.SaveAsync(permission.Actor, book);
    }

    public async Task EditLikeBookAsync(
        AdminOnlyPermission permission, Book book, User user, bool doLikeBook
    )
    {
        book.EditLike(permission, dateTimeProvider, user.ItemId, doLikeBook);
        await bookRepository.SaveAsync(permission.Actor, book);
    }
}
