using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;

namespace BookApi.Domain.Interfaces;

public interface IBookQueryService
{
    Task<BookResponseDTO?> FindByISBNAsync(Actor actor, ISBNCode isbn);
}

public interface IAuthorQueryService
{
    Task<AuthorResponseDTO?> FindAsync(IActor actor, int itemId);
}

public interface IPublisherQueryService
{
    Task<PublisherResponseDTO?> FindAsync(IActor actor, int itemId);
}

public interface IUserQueryService
{
    Task<UserResponseDTO?> FindByUserIdAsync(IActor actor, UserId userId);
}
