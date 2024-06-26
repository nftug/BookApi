using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Pagination;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Services.QueryServices;

public class AuthorQueryService(BookDbContext dbContext) : IAuthorQueryService
{
    public async Task<AuthorResponseDTO?> FindAsync(Actor? actor, ItemId itemId)
    {
        var response = await dbContext.Authors
            .Where(AuthorDataModel.QueryPredicate(actor))
            .Where(x => x.Id == itemId.Value)
            .Select(x => new AuthorResponseDTO(
                x.Id,
                x.Name,
                x.Books.Select(b => new BookSummaryResponseDTO(
                    b.ISBN,
                    b.Title,
                    b.PublishedAt,
                    b.Authors.Select(a => a.Id),
                    b.PublisherId
                ))
            ))
            .SingleOrDefaultAsync();

        return response is { }
            ? response with
            {
                RelatedPublishers =
                    (await dbContext.Books
                        .Where(b => b.Authors.Any(a => a.Id == itemId.Value))
                        .GroupBy(
                            b => b.PublisherId,
                            (k, g) => new PublisherSummaryResponseDTO(k, g.First().Publisher.Name)
                        )
                        .ToArrayAsync())
                        .OrderBy(x => x.PublisherId)
            }
            : null;
    }

    public async Task<PaginationResponseDTO<AuthorSummaryResponseDTO>> GetPaginatedResultsAsync(
        Actor? actor, AuthorQueryDTO queryFields
    )
    {
        var paginationQuery = new PaginationQuery(queryFields);

        var query = dbContext.Authors
            .Where(AuthorDataModel.QueryPredicate(actor))
            .Where(x => queryFields.Search == null || x.Name.Contains(queryFields.Search));

        int totalItems = await query.CountAsync();
        var results =
            await paginationQuery.PaginateQuery(query)
                .Select(x => new AuthorSummaryResponseDTO(x.Id, x.Name))
                .ToListAsync();

        return new(results, totalItems, paginationQuery);
    }
}
