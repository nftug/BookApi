using BookApi.Domain.Exceptions;

namespace BookApi.Domain.ValueObjects.Shared;

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

public record AuthorName : EntityNameBase<AuthorName>
{
    public override string FieldDisplayName => "著者名";
}

public record PublisherName : EntityNameBase<PublisherName>
{
    public override string FieldDisplayName => "出版社名";
}

public record BookTitle : EntityNameBase<BookTitle>
{
    public override string FieldDisplayName => "書名";
}
