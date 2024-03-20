using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Pagination;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;
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

    public async Task<PaginationResponseDTO<BookListItemResponseDTO>> GetPaginatedResultsAsync(
        Actor? actor, BookQueryDTO queryFields
    )
    {
        var paginationQuery = new PaginationQuery(queryFields);
        var query = ListQueryBase(actor, queryFields);
        return await GetPaginatedResultsAsyncCore(actor, paginationQuery, query);
    }

    public async Task<PaginationResponseDTO<BookListItemResponseDTO>> GetLikedBooksAsync(
        Actor? actor, UserId userId, BookQueryDTO queryFields
    )
    {
        var paginationQuery = new PaginationQuery(queryFields);
        var query = ListQueryBase(actor, queryFields)
            .Where(x => x.BookLikes.Any(x => x.User.UserId.ToLower() == userId.ToLower()))
            .OrderByDescending(x => x.BookLikes.Single(x => x.User.UserId.ToLower() == userId.ToLower()).LikedAt);

        return await GetPaginatedResultsAsyncCore(actor, paginationQuery, query);
    }

    public async Task<PaginationResponseDTO<BookLikeListItemResponseDTO>> GetLikesAsync(
        ISBNCode isbn, BookLikeQueryDTO queryFields
    )
    {
        var paginationQuery = new PaginationQuery(queryFields);
        var query = dbContext.BookLikes
            .Where(x => x.Book.ISBN == isbn.Value)
            .OrderByDescending(x => x.LikedAt);

        int totalItems = await query.CountAsync();
        var results =
            await paginationQuery.PaginateQuery(query)
                .Select(x => new BookLikeListItemResponseDTO(
                    new(x.User.UserId, x.User.UserName),
                    x.LikedAt
                ))
                .ToListAsync();

        return new(results, totalItems, paginationQuery);
    }

    private IQueryable<BookDataModel> ListQueryBase(Actor? actor, BookQueryDTO queryFields)
        => dbContext.Books
            .Where(BookDataModel.QueryPredicate(actor))
            .Where(x => queryFields.Search == null || x.Title.Contains(queryFields.Search))
            .Where(x => queryFields.AuthorId == null || x.Authors.Any(a => a.Id == queryFields.AuthorId))
            .Where(x => queryFields.PublisherId == null || x.PublisherId == queryFields.PublisherId);

    private static async Task<PaginationResponseDTO<BookListItemResponseDTO>> GetPaginatedResultsAsyncCore(
        Actor? actor, PaginationQuery paginationQuery, IQueryable<BookDataModel> query
    )
    {
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

        return new(results, totalItems, paginationQuery);
    }
}
