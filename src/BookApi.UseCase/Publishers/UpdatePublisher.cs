using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.Services;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Publishers;

public class UpdatePublisher
{
    public record Command(ActorForPermission Actor, int PublisherId, PublisherCommandDTO CommandForm) : IRequest;

    public class Handler(PublisherSaveService publisherSaveService, IPublisherRepository PublisherRepository) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var publisher =
                await PublisherRepository.FindAsync(request.Actor, request.PublisherId)
                ?? throw new ItemNotFoundException();

            var permission = new AdminOnlyPermission(request.Actor);
            await publisherSaveService.UpdateAsync(permission, publisher, request.CommandForm.Name);
        }
    }
}
