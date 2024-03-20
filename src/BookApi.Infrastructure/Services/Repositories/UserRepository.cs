using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Entities;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Users;
using BookApi.Infrastructure.Abstractions.Services;
using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Services.Repositories;

public class UserRepository(BookDbContext dbContext)
    : RepositoryBase<User, UserDataModel>(dbContext), IUserRepository
{
    protected override IQueryable<UserDataModel> QueryForRead(IActor actor)
        => DbContext.Users.Where(UserDataModel.QueryPredicate(actor));

    public async Task<User?> FindByUserIdAsync(UserId userId)
        => (await DbContext.Users
            .SingleOrDefaultAsync(x => x.UserId == userId.Value || x.UserId == userId.ToLower()))
            ?.ToEntity();

    public async Task<bool> AnyByUserIdAsync(UserId userId)
        => await DbContext.Users
            .AnyAsync(x => x.UserId == userId.Value || x.UserId == userId.ToLower());
}
