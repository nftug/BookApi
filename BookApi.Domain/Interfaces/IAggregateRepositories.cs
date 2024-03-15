using BookApi.Domain.Abstractions.Interfaces;
using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Entities;

namespace BookApi.Domain.Interfaces;

public interface IBookRepository : IRepositoryBase<Book>
{
    Task<Book?> FindByISBNAsync(IActor actor, string isbn);
    Task<bool> AnyByISBNAsync(string isbn);
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
