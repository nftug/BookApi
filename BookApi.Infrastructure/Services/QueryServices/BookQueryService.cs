using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Services.QueryServices;

public class BookQueryService(BookDbContext dbContext) : IBookQueryService
{
    public async Task<BookResponseDTO?> FindByISBNAsync(IActor actor, ISBNCode isbn)
        => await dbContext.Books
            .Where(BookDataModel.QueryPredicate(actor))
            .Where(x => x.ISBN == isbn.Value)
            .Select(x => new BookResponseDTO(
                x.ISBN,
                x.Title,
                x.PublishedAt,
                x.Authors.Select(a => new ItemSummaryResponseDTO(a.Id, a.Name)),
                new(x.PublisherId, x.Publisher.Name)
            ))
            .SingleOrDefaultAsync();
}
