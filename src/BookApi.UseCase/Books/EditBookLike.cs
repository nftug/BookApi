using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.Services;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;
using MediatR;

namespace BookApi.UseCase.Books;

public class EditBookLike
{
    public record Command(Actor Actor, string UserId, string ISBN, BookLikeEditCommandDTO FormCommand)
        : IRequest;

    public class Handler(
        BookLikeService bookLikeService, IBookRepository bookRepository, IUserRepository userRepository
    ) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var permission = new AdminOnlyPermission(request.Actor);

            var isbn = ISBNCode.CreateWithValidation(request.ISBN);
            var book =
                await bookRepository.FindByISBNAsync(request.Actor, isbn)
                ?? throw new ItemNotFoundException();

            var userId = UserId.Reconstruct(request.UserId);
            var user = await userRepository.FindByUserIdAsync(userId)
                ?? throw new ValidationErrorException($"ユーザーが見つかりません。");

            await bookLikeService.EditLikeBookAsync(permission, book, user, request.FormCommand.IsLiked);
        }
    }
}
