namespace BookApi.Domain.Abstractions.DTOs.Pagination;

public interface IPaginationQueryDTO
{
    int Page { get; }
    int Limit { get; }
}

public interface ISearchQueryDTO
{
    string? Search { get; }
}