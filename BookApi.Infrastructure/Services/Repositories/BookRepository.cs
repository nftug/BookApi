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
            .Include(x => x.Authors);

    public async Task<Book?> FindByISBNAsync(IActor actor, ISBNCode isbn)
        => (await QueryForRead(actor).SingleOrDefaultAsync(x => x.ISBN == isbn.Value))
            ?.ToEntity();

    public async Task<bool> AnyByISBNAsync(ISBNCode isbn)
        => await DbContext.Books.AnyAsync(x => x.ISBN == isbn.Value);
}
