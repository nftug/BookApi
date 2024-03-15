using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Books;

public class DeleteBook
{
    public record Command(ActorForPermission Actor, int BookID) : IRequest;

    public class Handler(IBookRepository bookRepository) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var book =
                await bookRepository.FindAsync(request.Actor, request.BookID)
                ?? throw new ItemNotFoundException();

            var permission = new AdminOnlyPermission(request.Actor);

            await bookRepository.DeleteAsync(permission, book.ID);
        }
    }
}
