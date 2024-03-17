using BookApi.Domain.Exceptions;

namespace BookApi.Domain.ValueObjects.Shared;

public record ItemId
{
    public int Value { get; }

    private ItemId(int value) => Value = value;

    public static ItemId CreateWithValidation(int value)
    {
        if (value <= 0)
            throw new ValidationErrorException("IDは1以上の数値を指定してください。");

        return new(value);
    }

    public static ItemId Reconstruct(int value) => new(value);

    public override string ToString() => Value.ToString();
}
