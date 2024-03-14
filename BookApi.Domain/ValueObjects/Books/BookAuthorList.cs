using System.Collections;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.ValueObjects.Books;

public class BookAuthorList : IEnumerable<ItemID>
{
    protected List<ItemID> Items { get; }

    private BookAuthorList(IEnumerable<ItemID> items) => Items = [.. items];

    public static BookAuthorList CreateWithValidation(int[] authorIDs)
        => new(authorIDs.Distinct().Select(ItemID.CreateWithValidation));

    public static BookAuthorList Reconstruct(int[] authorIDs)
        => new(authorIDs.Select(ItemID.Reconstruct));

    public IEnumerator<ItemID> GetEnumerator() => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
}
