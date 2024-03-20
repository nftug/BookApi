using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;
using MediatR;

namespace BookApi.UseCase.Books;

public class GetBookLikeList
{
    public record Query(Actor? Actor, string ISBN, BookLikeQueryDTO QueryFields)
        : IRequest<PaginationResponseDTO<BookLikeListItemResponseDTO>>;

    public class Handler(IBookQueryService bookQueryService)
        : IRequestHandler<Query, PaginationResponseDTO<BookLikeListItemResponseDTO>>
    {
        public async Task<PaginationResponseDTO<BookLikeListItemResponseDTO>> Handle(
            Query request, CancellationToken cancellationToken
        )
        {
            var isbn = ISBNCode.CreateWithValidation(request.ISBN);
            return await bookQueryService.GetLikesAsync(isbn, request.QueryFields);
        }
    }
}
