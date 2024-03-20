using BookApi.Domain.Abstractions.DTOs.Pagination;

namespace BookApi.Domain.DTOs.Queries;

public record BookQueryDTO : IPaginationQueryDTO, ISearchQueryDTO
{
    public int Page { get; init; } = 1;
    public int Limit { get; init; } = 10;
    public string? Search { get; init; }

    public int? AuthorId { get; init; }
    public int? PublisherId { get; init; }
}
