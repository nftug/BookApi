using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;
using MediatR;

namespace BookApi.UseCase.Books;

public class GetLikedBookList
{
    public record Query(Actor? Actor, string UserId, BookQueryDTO QueryFields)
       : IRequest<PaginationResponseDTO<BookListItemResponseDTO>>;

    public class Handler(IBookQueryService bookQueryService)
        : IRequestHandler<Query, PaginationResponseDTO<BookListItemResponseDTO>>
    {
        public async Task<PaginationResponseDTO<BookListItemResponseDTO>> Handle(
            Query request, CancellationToken cancellationToken
        )
        {
            var userId = UserId.Reconstruct(request.UserId);
            return await bookQueryService.GetLikedBooksAsync(request.Actor, userId, request.QueryFields);
        }
    }
}
