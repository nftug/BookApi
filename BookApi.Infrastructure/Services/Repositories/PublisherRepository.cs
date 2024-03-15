using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Entities;
using BookApi.Domain.Interfaces;
using BookApi.Infrastructure.Abstractions.Services;
using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Services.Repositories;

public class PublisherRepository(BookDbContext context)
    : RepositoryBase<Publisher, PublisherDataModel>(context), IPublisherRepository
{
    protected override IQueryable<PublisherDataModel> QueryForRead(IActor actor)
        => DbContext.Publishers
            .Where(PublisherDataModel.QueryPredicate(actor))
            .Include(x => x.Books);

    protected override IQueryable<PublisherDataModel> QueryForSave(IActor actor)
        => DbContext.Publishers
            .Where(PublisherDataModel.QueryPredicate(actor));

    public async Task<bool> AnyByNameAsync(string name)
        => await DbContext.Publishers.AnyAsync(x => x.Name == name);
}
