using BookApi.Domain.Exceptions;

namespace BookApi.Domain.Abstractions.ValueObjects;

public abstract record EntityNameBase<TSelf>
    where TSelf : EntityNameBase<TSelf>, new()
{
    public string Value { get; protected init; } = string.Empty;

    public abstract string FieldDisplayName { get; }

    protected EntityNameBase() { }

    public static TSelf CreateWithValidation(string? value)
    {
        var item = new TSelf { Value = value! };

        if (value is not { Length: > 0 })
            throw new ValidationErrorException($"{item.FieldDisplayName}は空にできません。");

        return item;
    }

    public static TSelf Reconstruct(string value) => new() { Value = value };

    public override string ToString() => Value;
}
