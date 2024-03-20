using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Services;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Books;

public class CreateBook
{
    public record Command(Actor Actor, BookCommandDTO CommandForm)
        : IRequest<BookCreationResponseDTO>;

    public class Handler(BookSaveService bookSaveService) : IRequestHandler<Command, BookCreationResponseDTO>
    {
        public async Task<BookCreationResponseDTO> Handle(Command request, CancellationToken cancellationToken)
        {
            var permission = new AdminOnlyPermission(request.Actor);

            var command = request.CommandForm;
            var newBook = await bookSaveService.CreateAsync(
                permission,
                command.Title, command.ISBN, command.AuthorIds, command.PublisherId, command.PublishedAt
            );

            return new(newBook.ISBN.Value);
        }
    }
}
