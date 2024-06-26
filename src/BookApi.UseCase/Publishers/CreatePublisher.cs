using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Services;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Publishers;

public class CreatePublisher
{
    public record Command(Actor Actor, PublisherCommandDTO CommandForm)
        : IRequest<ItemCreationResponseDTO>;

    public class Handler(PublisherSaveService publisherSaveService) : IRequestHandler<Command, ItemCreationResponseDTO>
    {
        public async Task<ItemCreationResponseDTO> Handle(Command request, CancellationToken cancellationToken)
        {
            var permission = new AdminOnlyPermission(request.Actor);
            var newPublisher = await publisherSaveService.CreateAsync(permission, request.CommandForm);
            return new(newPublisher.Id);
        }
    }
}
