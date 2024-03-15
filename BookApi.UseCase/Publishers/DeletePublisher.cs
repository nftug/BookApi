using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Publishers;

public class DeletePublisher
{
    public record Command(ActorForPermission Actor, int PublisherID) : IRequest;

    public class Handler(IPublisherRepository publisherRepository) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var permission = new AdminOnlyPermission(request.Actor);
            await publisherRepository.DeleteAsync(permission, request.PublisherID);
        }
    }
}
