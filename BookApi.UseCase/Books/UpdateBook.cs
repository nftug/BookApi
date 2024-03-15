using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.Services;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Books;

public class UpdateBook
{
    public record Command(ActorForPermission Actor, int BookID, BookCommandDTO CommandForm) : IRequest;

    public class Handler(BookSaveService bookSaveService, IBookRepository bookRepository) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var book =
                await bookRepository.FindAsync(request.Actor, request.BookID)
                ?? throw new ItemNotFoundException();

            var permission = new AdminOnlyPermission(request.Actor);

            var command = request.CommandForm;
            await bookSaveService.UpdateAsync(
                permission,
                book,
                command.Title, command.ISBN, command.AuthorIDs, command.PublisherID, command.PublishedAt
            );
        }
    }
}
