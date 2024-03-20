using BookApi.Domain.Abstractions.DTOs.Pagination;

namespace BookApi.Domain.DTOs.Queries;

public record BookLikeQueryDTO : IPaginationQueryDTO
{
    public int Page { get; init; } = 1;
    public int Limit { get; init; } = 10;
}
