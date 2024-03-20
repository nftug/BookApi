using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Services.QueryServices;

public class BookQueryService(BookDbContext dbContext) : IBookQueryService
{
    public async Task<BookResponseDTO?> FindByISBNAsync(Actor actor, ISBNCode isbn)
        => await dbContext.Books
            .Where(BookDataModel.QueryPredicate(actor))
            .Where(x => x.ISBN == isbn.Value)
            .Select(x => new BookResponseDTO(
                x.ISBN,
                x.Title,
                x.PublishedAt,
                x.BookAuthors
                    .OrderBy(x => x.Order)
                    .Select(x => new ItemSummaryResponseDTO(x.AuthorId, x.Author.Name)),
                new(x.PublisherId, x.Publisher.Name),
                x.BookLikes.Count(),
                x.BookLikes.Any(l => l.UserId == actor.Id.Value)
            ))
            .SingleOrDefaultAsync();
}
