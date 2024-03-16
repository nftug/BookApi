using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Authors;

public class GetAuthor
{
    public record Query(ActorForPermission Actor, int AuthorId) : IRequest<AuthorResponseDTO>;

    public class Handler(IAuthorQueryService authorQueryService) : IRequestHandler<Query, AuthorResponseDTO>
    {
        public async Task<AuthorResponseDTO> Handle(Query request, CancellationToken cancellationToken)
            => await authorQueryService.FindAsync(request.Actor, request.AuthorId)
                ?? throw new ItemNotFoundException();
    }
}
