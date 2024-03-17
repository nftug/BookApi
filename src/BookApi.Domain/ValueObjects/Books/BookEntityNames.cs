using BookApi.Domain.Abstractions.ValueObjects;

namespace BookApi.Domain.ValueObjects.Books;

public record AuthorName : EntityNameBase<AuthorName>
{
    protected override string FieldDisplayNameCore => "著者名";
    protected override int LimitLengthCore => 30;
}

public record PublisherName : EntityNameBase<PublisherName>
{
    protected override string FieldDisplayNameCore => "出版社名";
    protected override int LimitLengthCore => 20;
}

public record BookTitle : EntityNameBase<BookTitle>
{
    protected override string FieldDisplayNameCore => "書名";
    protected override int LimitLengthCore => 100;
}
