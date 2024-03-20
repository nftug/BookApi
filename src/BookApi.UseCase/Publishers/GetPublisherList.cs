using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Publishers;

public class GetPublisherList
{
    public record Query(Actor? Actor, PublisherQueryDTO QueryFields)
        : IRequest<PaginationResponseDTO<PublisherSummaryResponseDTO>>;

    public class Handler(IPublisherQueryService publisherQueryService)
    : IRequestHandler<Query, PaginationResponseDTO<PublisherSummaryResponseDTO>>
    {
        public async Task<PaginationResponseDTO<PublisherSummaryResponseDTO>> Handle(
            Query request, CancellationToken cancellationToken
        )
            => await publisherQueryService.GetPaginatedResultsAsync(request.Actor, request.QueryFields);
    }
}
