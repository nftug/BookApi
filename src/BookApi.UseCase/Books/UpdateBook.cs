using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.Services;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Books;

public class UpdateBook
{
    public record Command(Actor Actor, string ISBN, BookCommandDTO CommandForm) : IRequest;

    public class Handler(BookSaveService bookSaveService, IBookRepository bookRepository) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var isbnCode = ISBNCode.CreateWithValidation(request.ISBN);
            var book =
                await bookRepository.FindByISBNAsync(request.Actor, isbnCode)
                ?? throw new ItemNotFoundException();

            var permission = new AdminOnlyPermission(request.Actor);

            var command = request.CommandForm;
            await bookSaveService.UpdateAsync(
                permission,
                book,
                command.Title, command.ISBN, command.AuthorIds, command.PublisherId, command.PublishedAt
            );
        }
    }
}
