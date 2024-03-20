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

    Task<PaginationResponseDTO<BookListItemResponseDTO>> GetPaginatedResults(
        Actor? actor, BookQueryDTO queryFields
    );
}

public interface IAuthorQueryService
{
    Task<AuthorResponseDTO?> FindAsync(Actor? actor, int itemId);

    Task<PaginationResponseDTO<AuthorSummaryResponseDTO>> GetPaginatedResults(
        Actor? actor, AuthorQueryDTO queryFields
    );
}

public interface IPublisherQueryService
{
    Task<PublisherResponseDTO?> FindAsync(Actor? actor, int itemId);

    Task<PaginationResponseDTO<PublisherSummaryResponseDTO>> GetPaginatedResults(
        Actor? actor, PublisherQueryDTO queryFields
    );
}

public interface IUserQueryService
{
    Task<UserResponseDTO?> FindByUserIdAsync(Actor? actor, UserId userId);

    Task<PaginationResponseDTO<UserSummaryResponseDTO>> GetPaginatedResults(
        Actor? actor, UserQueryDTO queryFields
    );
}
