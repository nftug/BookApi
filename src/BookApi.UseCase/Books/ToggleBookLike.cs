using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.Services;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Books;

public class ToggleBookLike
{
    public record Command(Actor Actor, string ISBN) : IRequest<BookLikeResponseDTO>;

    public class Handler(BookLikeService bookLikeService, IBookRepository bookRepository)
        : IRequestHandler<Command, BookLikeResponseDTO>
    {
        public async Task<BookLikeResponseDTO> Handle(Command request, CancellationToken cancellationToken)
        {
            var permission = new PassThroughPermission(request.Actor);
            var isbn = ISBNCode.CreateWithValidation(request.ISBN);

            var book =
                await bookRepository.FindByISBNAsync(request.Actor, isbn)
                ?? throw new ItemNotFoundException();

            await bookLikeService.ToggleLikeBookAsync(permission, book);

            return new(book.IsLikedByMe(request.Actor));
        }
    }
}
