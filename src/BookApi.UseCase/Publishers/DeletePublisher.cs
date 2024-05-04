using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Publishers;

public class DeletePublisher
{
    public record Command(Actor Actor, int PublisherId) : IRequest;

    public class Handler(IPublisherRepository publisherRepository) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var permission = new AdminOnlyPermission(request.Actor);

            var publisher =
                await publisherRepository.FindAsync(permission.Actor, ItemId.CreateWithValidation(request.PublisherId))
                ?? throw new ItemNotFoundException();

            await publisherRepository.DeleteAsync(permission, publisher);
        }
    }
}
