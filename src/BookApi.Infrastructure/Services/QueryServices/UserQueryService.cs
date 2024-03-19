using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Users;
using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Services.QueryServices;

public class UserQueryService(BookDbContext dbContext) : IUserQueryService
{
    public async Task<UserResponseDTO?> FindByUserIdAsync(IActor actor, UserId userId)
        => await dbContext.Users
            .Where(UserDataModel.QueryPredicate(actor))
            .Where(x => x.UserId == userId.Value)
            .Select(x => new UserResponseDTO(
                x.UserId,
                x.UserName,
                x.CreatedAt,
                x.BookLikes.Count()
            ))
            .SingleOrDefaultAsync();
}
