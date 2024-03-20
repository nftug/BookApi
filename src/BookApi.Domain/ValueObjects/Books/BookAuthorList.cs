using System.Collections;
using BookApi.Domain.Exceptions;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.ValueObjects.Books;

public class BookAuthorList : IEnumerable<ItemId>
{
    private readonly List<ItemId> _items;

    private BookAuthorList(IEnumerable<ItemId> items) => _items = [.. items];

    public static BookAuthorList CreateWithValidation(int[] authorIds)
    {
        if (authorIds.Length == 0)
            throw new ValidationErrorException("著者を1名以上指定してください。");

        return new(authorIds.Distinct().Select(ItemId.CreateWithValidation));
    }

    public static BookAuthorList Reconstruct(int[] authorIds)
        => new(authorIds.Select(ItemId.Reconstruct));

    public IEnumerator<ItemId> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
}
