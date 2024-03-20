using BookApi.Domain.Abstractions.DTOs.Pagination;
using BookApi.Domain.Exceptions;

namespace BookApi.Domain.ValueObjects.Pagination;

public class PaginationQuery
{
    public IPaginationQueryDTO Query { get; }

    public int Page => Query.Page;
    public int Limit => Query.Limit;
    public int StartIndex { get; }

    public PaginationQuery(IPaginationQueryDTO query)
    {
        if (query.Page <= 0)
            throw new ValidationErrorException("ページ数が不正です。");

        Query = query;
        StartIndex = (Query.Page - 1) * Query.Limit;
    }

    public IQueryable<T> PaginateQuery<T>(IQueryable<T> query)
        => query.Skip(StartIndex).Take(Limit);
}
