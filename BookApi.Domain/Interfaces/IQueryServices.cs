using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Responses;

namespace BookApi.Domain.Interfaces;

public interface IBookQueryService
{
    Task<BookResponseDTO?> FindByISBNAsync(IActor actor, string isbn);
}

public interface IAuthorQueryService
{
    Task<AuthorResponseDTO?> FindAsync(IActor actor, int itemId);
}

public interface IPublisherQueryService
{
    Task<PublisherResponseDTO?> FindAsync(IActor actor, int itemId);
}
