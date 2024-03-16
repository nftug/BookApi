using BookApi.Domain.Exceptions;

namespace BookApi.Domain.Abstractions.ValueObjects;

public abstract record EntityNameBase<TSelf>
    where TSelf : EntityNameBase<TSelf>, new()
{
    public string Value { get; protected init; } = string.Empty;

    protected abstract string FieldDisplayNameCore { get; }
    protected virtual int LimitLengthCore => 200;

    protected EntityNameBase() { }

    public static readonly string FieldDisplayName = new TSelf().FieldDisplayNameCore;
    public static readonly int LimitLength = new TSelf().LimitLengthCore;

    public static TSelf CreateWithValidation(string? value)
    {
        if (value is not { Length: > 0 })
            throw new ValidationErrorException($"{FieldDisplayName}は空にできません。");
        if (value.Length > LimitLength)
            throw new ValidationErrorException($"{FieldDisplayName}は{LimitLength}文字以内で入力してください。");

        return new() { Value = value };
    }

    public static TSelf Reconstruct(string value) => new() { Value = value };

    public override string ToString() => Value;
}
