using BookApi.Domain.Abstractions.ValueObjects;

namespace BookApi.Domain.ValueObjects.Books;

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
