using BookApi.Domain.Abstractions.Interfaces;
using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Entities;
using BookApi.Domain.ValueObjects.Books;

namespace BookApi.Domain.Interfaces;

public interface IBookRepository : IRepositoryBase<Book>
{
    Task<Book?> FindByISBNAsync(IActor actor, ISBNCode isbn);
    Task<bool> AnyByISBNAsync(ISBNCode isbn, ISBNCode? isbnExcluded = null);
}

public interface IAuthorRepository : IRepositoryBase<Author>
{
    Task<bool> IsAllIdsExistedAsync(IActor actor, HashSet<int> itemIds);
    Task<bool> AnyByNameAsync(string name, int? itemIdExcluded = null);
}

public interface IPublisherRepository : IRepositoryBase<Publisher>
{
    Task<bool> AnyByNameAsync(string name, int? itemIdExcluded = null);
}
