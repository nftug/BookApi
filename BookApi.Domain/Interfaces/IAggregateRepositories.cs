using BookApi.Domain.Abstractions.Interfaces;
using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Entities;
using BookApi.Domain.ValueObjects.Books;

namespace BookApi.Domain.Interfaces;

public interface IBookRepository : IRepositoryBase<Book>
{
    Task<Book?> FindByISBNAsync(IActor actor, ISBNCode isbn);
    Task<bool> AnyByISBNAsync(ISBNCode isbn);
}

public interface IAuthorRepository : IRepositoryBase<Author>
{
    Task<bool> IsAllIDsExistedAsync(IActor actor, HashSet<int> itemIDs);
    Task<bool> AnyByNameAsync(string name);
}

public interface IPublisherRepository : IRepositoryBase<Publisher>
{
    Task<bool> AnyByNameAsync(string name);
}
