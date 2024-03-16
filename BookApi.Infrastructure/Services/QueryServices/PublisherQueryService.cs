using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Services.QueryServices;

public class PublisherQueryService(BookDbContext dbContext) : IPublisherQueryService
{
    public async Task<PublisherResponseDTO?> FindAsync(IActor actor, int itemId)
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
                        .Select(a => new ItemSummaryResponseDTO(a.Id, a.Name))
                        .ToArrayAsync())
                        .Distinct()
                        .OrderBy(x => x.Id)
            }
            : null;
    }
}
