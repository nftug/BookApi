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
            .Where(x => x.ID == itemId)
            .Select(x => new PublisherResponseDTO(
                x.ID,
                x.Name,
                x.Books.Select(b => new BookSummaryResponseDTO(
                    b.ISBN,
                    b.Title,
                    b.PublishedAt,
                    b.Authors.Select(a => a.ID),
                    b.PublisherID
                ))
            ))
            .SingleOrDefaultAsync();

        return response is { }
            ? response with
            {
                RelatedAuthors =
                    (await dbContext.Books
                        .Where(b => b.PublisherID == itemId)
                        .SelectMany(b => b.Authors)
                        .Select(a => new ItemSummaryResponseDTO(a.ID, a.Name))
                        .ToArrayAsync())
                        .Distinct()
                        .OrderBy(x => x.ID)
            }
            : null;
    }
}
