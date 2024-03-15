using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Entities;
using BookApi.Domain.Interfaces;
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
            .Include(x => x.Authors);

    protected override IQueryable<BookDataModel> QueryForSave(IActor actor)
        => DbContext.Books
            .Where(BookDataModel.QueryPredicate(actor))
            .Include(x => x.BookAuthors);

    public async Task<Book?> FindByISBNAsync(IActor actor, string isbn)
        => (await QueryForRead(actor).Where(x => x.ISBN == isbn).SingleOrDefaultAsync())
            ?.ToEntity();

    public async Task<bool> AnyByISBNAsync(string isbn)
        => await DbContext.Books.AnyAsync(x => x.ISBN == isbn);
}
