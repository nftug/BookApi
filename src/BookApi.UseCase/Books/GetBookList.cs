using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Books;

public class GetBookList
{
    public record Query(Actor? Actor, BookQueryDTO QueryFields)
        : IRequest<PaginationResponseDTO<BookListItemResponseDTO>>;

    public class Handler(IBookQueryService bookQueryService)
    : IRequestHandler<Query, PaginationResponseDTO<BookListItemResponseDTO>>
    {
        public async Task<PaginationResponseDTO<BookListItemResponseDTO>> Handle(
            Query request, CancellationToken cancellationToken
        )
            => await bookQueryService.GetPaginatedResults(request.Actor, request.QueryFields);
    }
}
