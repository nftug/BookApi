using BookApi.Domain.Abstractions.Interfaces;
using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Entities;

namespace BookApi.Domain.Interfaces;

public interface IBookRepository : IRepositoryBase<Book>
{
    Task<Book?> FindByISBNAsync(IActor actor, string isbn);
}

public interface IAuthorRepository : IRepositoryBase<Author>
{
}

public interface IPublisherRepository : IRepositoryBase<Publisher>
{
}
