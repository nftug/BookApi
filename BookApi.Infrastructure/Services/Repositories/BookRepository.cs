using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Entities;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Infrastructure.Abstractions.Services;
using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Services.Repositories;

public class BookRepository(BookDbContext context)
    : RepositoryBase<Book, BookDataModel>(context), IBookRepository
{
    protected override IQueryable<BookDataModel> QueryForRead(IActor actor)
        => DbContext.Books
            .Where(BookDataModel.QueryPredicate(actor))
            .Include(x => x.BookAuthors)
            .ThenInclude(x => x.Author);

    public virtual async Task<Book?> FindByISBNAsync(IActor actor, ISBNCode isbn)
        => (await QueryForRead(actor).SingleOrDefaultAsync(x => x.ISBN == isbn.Value))
            ?.ToEntity();

    public async Task<bool> AnyByISBNAsync(ISBNCode isbn, ISBNCode? isbnExcluded = null)
        => await DbContext.Books
            .Where(x => isbnExcluded == null || x.ISBN != isbnExcluded.Value)
            .AnyAsync(x => x.ISBN == isbn.Value);
}
