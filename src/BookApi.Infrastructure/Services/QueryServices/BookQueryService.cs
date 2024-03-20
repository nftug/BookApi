using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Pagination;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Services.QueryServices;

public class BookQueryService(BookDbContext dbContext) : IBookQueryService
{
    public async Task<BookResponseDTO?> FindByISBNAsync(Actor? actor, ISBNCode isbn)
        => await dbContext.Books
            .Where(BookDataModel.QueryPredicate(actor))
            .Where(x => x.ISBN == isbn.Value)
            .Select(x => new BookResponseDTO(
                x.ISBN,
                x.Title,
                x.PublishedAt,
                x.BookAuthors
                    .OrderBy(x => x.Order)
                    .Select(x => new AuthorSummaryResponseDTO(x.AuthorId, x.Author.Name)),
                new(x.PublisherId, x.Publisher.Name),
                x.BookLikes.Count(),
                actor != null && x.BookLikes.Any(l => l.UserId == actor.Id.Value)
            ))
            .SingleOrDefaultAsync();

    public async Task<PaginationResponseDTO<BookListItemResponseDTO>> GetPaginatedResults(
        Actor? actor, BookQueryDTO queryFields
    )
    {
        var paginationQuery = new PaginationQuery(queryFields);

        var query = dbContext.Books
            .Where(BookDataModel.QueryPredicate(actor))
            .Where(x => queryFields.Search == null || x.Title.Contains(queryFields.Search))
            .Where(x => queryFields.AuthorId == null || x.Authors.Any(a => a.Id == queryFields.AuthorId))
            .Where(x => queryFields.PublisherId == null || x.PublisherId == queryFields.PublisherId);

        int totalItems = await query.CountAsync();
        var results =
            await paginationQuery.PaginateQuery(query)
                .Select(x => new BookListItemResponseDTO(
                    x.ISBN,
                    x.Title,
                    x.PublishedAt,
                    x.BookAuthors
                        .OrderBy(x => x.Order)
                        .Select(x => new AuthorSummaryResponseDTO(x.AuthorId, x.Author.Name)),
                    new(x.PublisherId, x.Publisher.Name),
                    x.BookLikes.Count(),
                    actor != null && x.BookLikes.Any(l => l.UserId == actor.Id.Value)
                ))
                .ToListAsync();

        return new(results, totalItems, queryFields);
    }
}
