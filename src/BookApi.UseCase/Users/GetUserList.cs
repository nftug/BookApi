using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Users;

public class GetUserList
{
    public record Query(Actor? Actor, UserQueryDTO QueryFields)
        : IRequest<PaginationResponseDTO<UserSummaryResponseDTO>>;

    public class Handler(IUserQueryService userQueryService)
    : IRequestHandler<Query, PaginationResponseDTO<UserSummaryResponseDTO>>
    {
        public async Task<PaginationResponseDTO<UserSummaryResponseDTO>> Handle(
            Query request, CancellationToken cancellationToken
        )
            => await userQueryService.GetPaginatedResults(request.Actor, request.QueryFields);
    }
}
