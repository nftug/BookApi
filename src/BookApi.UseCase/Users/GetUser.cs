using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;
using MediatR;

namespace BookApi.UseCase.Users;

public class GetUser
{
    public record Query(Actor? Actor, string UserId) : IRequest<UserResponseDTO>;

    public class Handler(IUserQueryService userQueryService) : IRequestHandler<Query, UserResponseDTO>
    {
        public async Task<UserResponseDTO> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = UserId.Reconstruct(request.UserId);
            return
                await userQueryService.FindByUserIdAsync(request.Actor, userId)
                ?? throw new ItemNotFoundException();
        }
    }
}
