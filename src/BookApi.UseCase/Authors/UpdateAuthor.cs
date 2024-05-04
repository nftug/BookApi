using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.Services;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Authors;

public class UpdateAuthor
{
    public record Command(Actor Actor, int AuthorId, AuthorCommandDTO CommandForm) : IRequest;

    public class Handler(AuthorSaveService authorSaveService, IAuthorRepository authorRepository) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var author =
                await authorRepository.FindAsync(request.Actor, request.AuthorId)
                ?? throw new ItemNotFoundException();

            var permission = new AdminOnlyPermission(request.Actor);
            await authorSaveService.UpdateAsync(permission, author, request.CommandForm);
        }
    }
}
