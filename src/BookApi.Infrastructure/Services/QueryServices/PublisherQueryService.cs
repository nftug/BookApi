using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Pagination;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Services.QueryServices;

public class PublisherQueryService(BookDbContext dbContext) : IPublisherQueryService
{
    public async Task<PublisherResponseDTO?> FindAsync(Actor? actor, int itemId)
    {
        var response = await dbContext.Publishers
            .Where(PublisherDataModel.QueryPredicate(actor))
            .Where(x => x.Id == itemId)
            .Select(x => new PublisherResponseDTO(
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
                RelatedAuthors =
                    (await dbContext.Books
                        .Where(b => b.PublisherId == itemId)
                        .SelectMany(b => b.Authors)
                        .GroupBy(
                            a => a.Id,
                            (k, g) => new AuthorSummaryResponseDTO(k, g.First().Name)
                        )
                        .ToArrayAsync())
                        .OrderBy(x => x.AuthorId)
            }
            : null;
    }

    public async Task<PaginationResponseDTO<PublisherSummaryResponseDTO>> GetPaginatedResultsAsync(
        Actor? actor, PublisherQueryDTO queryFields
    )
    {
        var paginationQuery = new PaginationQuery(queryFields);

        var query = dbContext.Publishers
            .Where(PublisherDataModel.QueryPredicate(actor))
            .Where(x => queryFields.Search == null || x.Name.Contains(queryFields.Search));

        int totalItems = await query.CountAsync();
        var results =
            await paginationQuery.PaginateQuery(query)
                .Select(x => new PublisherSummaryResponseDTO(x.Id, x.Name))
                .ToListAsync();

        return new(results, totalItems, paginationQuery);
    }
}
