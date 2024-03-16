using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Publishers;

public class GetPublisher
{
    public record Query(ActorForPermission Actor, int PublisherId) : IRequest<PublisherResponseDTO>;

    public class Handler(IPublisherQueryService publisherQueryService) : IRequestHandler<Query, PublisherResponseDTO>
    {
        public async Task<PublisherResponseDTO> Handle(Query request, CancellationToken cancellationToken)
            => await publisherQueryService.FindAsync(request.Actor, request.PublisherId)
                ?? throw new ItemNotFoundException();
    }
}
