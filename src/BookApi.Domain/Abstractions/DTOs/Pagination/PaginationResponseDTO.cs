using BookApi.Domain.Abstractions.DTOs.Pagination;

namespace BookApi.Domain.Abstractions.DTOs;

public class PaginationResponseDTO<T>
{
    public int TotalItems { get; init; }
    public int CurrentPage { get; init; }
    public int? NextPage { get; init; }
    public int? PreviousPage { get; init; }
    public int TotalPages { get; init; }
    public IEnumerable<T> Results { get; init; }

    public PaginationResponseDTO(IEnumerable<T> list, int totalItems, IPaginationQueryDTO queryFields)
    {
        TotalItems = totalItems;
        TotalPages = queryFields.Limit is > 0
            ? (int)Math.Ceiling((double)totalItems / queryFields.Limit)
            : 0;
        CurrentPage = queryFields.Page;
        NextPage = CurrentPage < TotalPages ? CurrentPage + 1 : null;
        PreviousPage = CurrentPage > 1 ? CurrentPage - 1 : null;
        Results = list;
    }
}
