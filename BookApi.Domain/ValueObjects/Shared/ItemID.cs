using BookApi.Domain.Exceptions;

namespace BookApi.Domain.ValueObjects.Shared;

public record ItemID
{
    public int Value { get; }

    private ItemID(int value) => Value = value;

    public static ItemID CreateWithValidation(int value)
    {
        if (value <= 0)
            throw new ValidationErrorException("IDは1以上の数値を指定してください。");

        return new(value);
    }

    public static ItemID Reconstruct(int value) => new(value);

    public override string ToString() => Value.ToString();
}
