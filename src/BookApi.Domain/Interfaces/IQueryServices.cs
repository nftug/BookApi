using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;

namespace BookApi.Domain.Interfaces;

public interface IBookQueryService
{
    Task<BookResponseDTO?> FindByISBNAsync(Actor actor, ISBNCode isbn);

    Task<PaginationResponseDTO<BookListItemResponseDTO>> GetPaginatedResults(
        Actor actor, BookQueryDTO queryFields
    );
}

public interface IAuthorQueryService
{
    Task<AuthorResponseDTO?> FindAsync(IActor actor, int itemId);

    Task<PaginationResponseDTO<AuthorSummaryResponseDTO>> GetPaginatedResults(
        IActor actor, AuthorQueryDTO queryFields
    );
}

public interface IPublisherQueryService
{
    Task<PublisherResponseDTO?> FindAsync(IActor actor, int itemId);

    Task<PaginationResponseDTO<PublisherSummaryResponseDTO>> GetPaginatedResults(
        IActor actor, PublisherQueryDTO queryFields
    );
}

public interface IUserQueryService
{
    Task<UserResponseDTO?> FindByUserIdAsync(IActor actor, UserId userId);

    Task<PaginationResponseDTO<UserSummaryResponseDTO>> GetPaginatedResults(
        IActor actor, UserQueryDTO queryFields
    );
}
