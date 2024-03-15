using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Services.QueryServices;

public class AuthorQueryService(BookDbContext dbContext) : IAuthorQueryService
{
    public async Task<AuthorResponseDTO?> FindAsync(IActor actor, int itemId)
    {
        var response = await dbContext.Authors
            .Where(AuthorDataModel.QueryPredicate(actor))
            .Where(x => x.ID == itemId)
            .Select(x => new AuthorResponseDTO(
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
                RelatedPublishers =
                    (await dbContext.Books
                        .Where(b => b.Authors.Any(a => a.ID == itemId))
                        .Select(b => new ItemSummaryResponseDTO(b.PublisherID, b.Publisher.Name))
                        .ToArrayAsync())
                        .Distinct()
                        .OrderBy(x => x.ID)
            }
            : null;
    }
}
