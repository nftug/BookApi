using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Authors;

public class DeleteAuthor
{
    public record Command(ActorForPermission Actor, int AuthorID) : IRequest;

    public class Handler(IAuthorRepository authorRepository) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var permission = new AdminOnlyPermission(request.Actor);
            await authorRepository.DeleteAsync(permission, request.AuthorID);
        }
    }
}
