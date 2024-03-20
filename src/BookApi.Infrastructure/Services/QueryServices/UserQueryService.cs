using BookApi.Domain.Abstractions.DTOs;
using BookApi.Domain.DTOs.Queries;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Pagination;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;
using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Services.QueryServices;

public class UserQueryService(BookDbContext dbContext) : IUserQueryService
{
    public async Task<UserResponseDTO?> FindByUserIdAsync(Actor? actor, UserId userId)
        => await dbContext.Users
            .Where(UserDataModel.QueryPredicate(actor))
            .Where(x => x.UserId.ToLower() == userId.ToLower())
            .Select(x => new UserResponseDTO(
                x.UserId,
                x.UserName,
                x.CreatedAt,
                x.BookLikes.Count()
            ))
            .SingleOrDefaultAsync();

    public async Task<PaginationResponseDTO<UserSummaryResponseDTO>> GetPaginatedResultsAsync(
        Actor? actor, UserQueryDTO queryFields
    )
    {
        var paginationQuery = new PaginationQuery(queryFields);

        var query = dbContext.Users
            .Where(UserDataModel.QueryPredicate(actor))
            .Where(x =>
                queryFields.Search == null
                || x.UserName.Contains(queryFields.Search)
                || x.UserId.Contains(queryFields.Search));

        int totalItems = await query.CountAsync();
        var results =
            await paginationQuery.PaginateQuery(query)
                .Select(x => new UserSummaryResponseDTO(x.UserId, x.UserName))
                .ToListAsync();

        return new(results, totalItems, paginationQuery);
    }
}
