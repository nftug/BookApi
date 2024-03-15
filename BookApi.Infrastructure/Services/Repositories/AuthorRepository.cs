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
}
