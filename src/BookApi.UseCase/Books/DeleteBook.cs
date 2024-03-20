using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Books;

public class DeleteBook
{
    public record Command(Actor Actor, string ISBN) : IRequest;

    public class Handler(IBookRepository bookRepository) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var isbnCode = ISBNCode.CreateWithValidation(request.ISBN);
            var book =
                await bookRepository.FindByISBNAsync(request.Actor, isbnCode)
                ?? throw new ItemNotFoundException();

            var permission = new AdminOnlyPermission(request.Actor);

            await bookRepository.DeleteAsync(permission, book);
        }
    }
}
