using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.ValueObjects.Books;

namespace BookApi.Domain.Interfaces;

public interface IBookQueryService
{
    Task<BookResponseDTO?> FindByISBNAsync(IActor actor, ISBNCode isbn);
}

public interface IAuthorQueryService
{
    Task<AuthorResponseDTO?> FindAsync(IActor actor, int itemId);
}

public interface IPublisherQueryService
{
    Task<PublisherResponseDTO?> FindAsync(IActor actor, int itemId);
}
