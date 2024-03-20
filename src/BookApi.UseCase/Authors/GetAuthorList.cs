using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Authors;

public class GetAuthorList
{
    public record Query(Actor? Actor, AuthorQueryDTO QueryFields)
        : IRequest<PaginationResponseDTO<AuthorSummaryResponseDTO>>;

    public class Handler(IAuthorQueryService authorQueryService)
    : IRequestHandler<Query, PaginationResponseDTO<AuthorSummaryResponseDTO>>
    {
        public async Task<PaginationResponseDTO<AuthorSummaryResponseDTO>> Handle(
            Query request, CancellationToken cancellationToken
        )
            => await authorQueryService.GetPaginatedResults(request.Actor, request.QueryFields);
    }
}
