using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Entities;
using BookApi.Domain.Interfaces;
using BookApi.Infrastructure.Abstractions.Services;
using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Services.Repositories;

public class AuthorRepository(BookDbContext context)
    : RepositoryBase<Author, AuthorDataModel>(context), IAuthorRepository
{
    protected override IQueryable<AuthorDataModel> QueryForRead(IActor actor)
        => DbContext.Authors
            .Where(AuthorDataModel.QueryPredicate(actor))
            .Include(x => x.Books);

    public async Task<bool> IsAllIdsExistedAsync(IActor actor, HashSet<int> itemIds)
    {
        var existingIds =
            await DbContext.Authors
                .Where(AuthorDataModel.QueryPredicate(actor))
                .Where(x => itemIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

        return itemIds.All(existingIds.Contains);
    }

    public async Task<bool> AnyByNameAsync(string name)
        => await DbContext.Authors.AnyAsync(x => x.Name == name);
}
