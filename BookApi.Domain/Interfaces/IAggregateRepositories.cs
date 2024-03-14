using BookApi.Domain.Abstractions.Interfaces;
using BookApi.Domain.Entities;

namespace BookApi.Domain.Interfaces;

public interface IBookRepository : IRepositoryBase<Book>
{
}

public interface IAuthorRepository : IRepositoryBase<Author>
{
}

public interface IPublisherRepository : IRepositoryBase<Publisher>
{
}
