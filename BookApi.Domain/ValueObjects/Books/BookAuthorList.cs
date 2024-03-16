using System.Collections;
using BookApi.Domain.Exceptions;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.ValueObjects.Books;

public class BookAuthorList : IEnumerable<ItemId>
{
    protected List<ItemId> Items { get; }

    private BookAuthorList(IEnumerable<ItemId> items) => Items = [.. items];

    public static BookAuthorList CreateWithValidation(int[] authorIds)
    {
        if (authorIds.Length == 0)
            throw new ValidationErrorException("著者を1名以上指定してください。");

        return new(authorIds.Distinct().Select(ItemId.CreateWithValidation));
    }

    public static BookAuthorList Reconstruct(int[] authorIds)
        => new(authorIds.Select(ItemId.Reconstruct));

    public IEnumerator<ItemId> GetEnumerator() => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
}
