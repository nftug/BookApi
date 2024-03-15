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

    protected override IQueryable<AuthorDataModel> QueryForSave(IActor actor)
        => DbContext.Authors
            .Where(AuthorDataModel.QueryPredicate(actor));

    public async Task<bool> IsAllIDsExistedAsync(IActor actor, HashSet<int> itemIDs)
    {
        var existingIDs =
            await DbContext.Authors
                .Where(AuthorDataModel.QueryPredicate(actor))
                .Where(x => itemIDs.Contains(x.ID))
                .Select(x => x.ID)
                .ToListAsync();

        return itemIDs.All(existingIDs.Contains);
    }

    public async Task<bool> AnyByNameAsync(string name)
        => await DbContext.Authors.AnyAsync(x => x.Name == name);
}
