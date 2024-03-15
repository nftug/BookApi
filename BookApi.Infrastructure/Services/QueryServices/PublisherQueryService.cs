using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Services.QueryServices;

public class PublisherQueryService(BookDbContext dbContext) : IPublisherQueryService
{
    public async Task<PublisherResponseDTO?> FindAsync(IActor actor, int itemId)
        => await dbContext.Publishers
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
                )),
                x.Books
                    .SelectMany(b => b.Authors)
                    .Select(a => new ItemSummaryResponseDTO(a.ID, a.Name))
                    .DistinctBy(x => x.ID)
            ))
            .SingleOrDefaultAsync();
}
