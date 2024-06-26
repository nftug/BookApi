using BookApi.Domain.Abstractions.Interfaces;
using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Entities;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;

namespace BookApi.Domain.Interfaces;

public interface IBookRepository : IRepositoryBase<Book>
{
    Task<Book?> FindByISBNAsync(IActor actor, ISBNCode isbn);
    Task<bool> AnyByISBNAsync(ISBNCode isbn, ISBNCode? isbnExcluded = null);
}

public interface IAuthorRepository : IRepositoryBase<Author>
{
    Task<bool> IsAllIdsExistedAsync(IActor actor, HashSet<ItemId> itemIds);
    Task<bool> AnyByNameAsync(string name, ItemId? itemIdExcluded = null);
}

public interface IPublisherRepository : IRepositoryBase<Publisher>
{
    Task<bool> AnyByNameAsync(string name, ItemId? itemIdExcluded = null);
}

public interface IUserRepository : IRepositoryBase<User>
{
    Task<User?> FindByUserIdAsync(UserId userId);
    Task<bool> AnyByUserIdAsync(UserId userId);
}
