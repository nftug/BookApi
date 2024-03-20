using BookApi.Domain.Abstractions.DTOs.Pagination;
using BookApi.Domain.ValueObjects.Pagination;

namespace BookApi.Domain.Abstractions.DTOs;

public class PaginationResponseDTO<T>
{
    public int TotalItems { get; init; }
    public int CurrentPage { get; init; }
    public int? NextPage { get; init; }
    public int? PreviousPage { get; init; }
    public int TotalPages { get; init; }
    public IEnumerable<T> Results { get; init; }

    public PaginationResponseDTO(IEnumerable<T> list, int totalItems, PaginationQuery paginationQuery)
    {
        TotalItems = totalItems;
        TotalPages = paginationQuery.Limit is > 0
            ? (int)Math.Ceiling((double)totalItems / paginationQuery.Limit)
            : 0;
        CurrentPage = paginationQuery.Page;
        NextPage = CurrentPage < TotalPages ? CurrentPage + 1 : null;
        PreviousPage = CurrentPage > 1 ? CurrentPage - 1 : null;
        Results = list;
    }
}
