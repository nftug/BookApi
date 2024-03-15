using BookApi.Domain.Exceptions;

namespace BookApi.Domain.ValueObjects.Books;

public record BookPublicationDate
{
    public DateTime Value { get; }

    private BookPublicationDate(DateTime value) => Value = value;

    public static BookPublicationDate CreateWithValidation(DateTime value)
    {
        if (value == default)
            throw new ValidationErrorException("出版日が不正です。");

        return new(value);
    }

    public static BookPublicationDate Reconstruct(DateTime value) => new(value);

    public override string ToString() => Value.ToLocalTime().ToString();
}
