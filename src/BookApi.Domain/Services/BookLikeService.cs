using BookApi.Domain.Entities;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;

namespace BookApi.Domain.Services;

public class BookLikeService(
    IBookRepository bookRepository, IUserRepository userRepository, IDateTimeProvider dateTimeProvider
)
{
    public async Task ToggleLikeBookAsync(PassThroughPermission permission, Book book)
    {
        book.ToggleLike(permission, dateTimeProvider);
        await bookRepository.SaveAsync(permission.Actor, book);
    }

    public async Task EditLikeBookAsync(
        AdminOnlyPermission permission, Book book, UserId userId, bool doLikeBook
    )
    {
        var user = await userRepository.FindByUserIdAsync(userId)
            ?? throw new ValidationErrorException("指定されたユーザーが見つかりません。");

        book.EditLike(permission, dateTimeProvider, user.ItemId, doLikeBook);
        await bookRepository.SaveAsync(permission.Actor, book);
    }
}
