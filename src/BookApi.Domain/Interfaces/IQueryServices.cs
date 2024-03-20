using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;

namespace BookApi.Domain.Interfaces;

public interface IBookQueryService
{
    Task<BookResponseDTO?> FindByISBNAsync(Actor? actor, ISBNCode isbn);

    Task<PaginationResponseDTO<BookListItemResponseDTO>> GetPaginatedResultsAsync(
        Actor? actor, BookQueryDTO queryFields
    );

    Task<PaginationResponseDTO<BookLikeListItemResponseDTO>> GetLikesAsync(
        ISBNCode isbn, BookLikeQueryDTO queryFields
    );

    Task<PaginationResponseDTO<BookListItemResponseDTO>> GetLikedBooksAsync(
        Actor? actor, UserId userId, BookQueryDTO queryFields
    );
}

public interface IAuthorQueryService
{
    Task<AuthorResponseDTO?> FindAsync(Actor? actor, int itemId);

    Task<PaginationResponseDTO<AuthorSummaryResponseDTO>> GetPaginatedResultsAsync(
        Actor? actor, AuthorQueryDTO queryFields
    );
}

public interface IPublisherQueryService
{
    Task<PublisherResponseDTO?> FindAsync(Actor? actor, int itemId);

    Task<PaginationResponseDTO<PublisherSummaryResponseDTO>> GetPaginatedResultsAsync(
        Actor? actor, PublisherQueryDTO queryFields
    );
}

public interface IUserQueryService
{
    Task<UserResponseDTO?> FindByUserIdAsync(Actor? actor, UserId userId);

    Task<PaginationResponseDTO<UserSummaryResponseDTO>> GetPaginatedResultsAsync(
        Actor? actor, UserQueryDTO queryFields
    );
}
