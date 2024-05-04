using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using BookApi.Domain.Exceptions;

namespace BookApi.Domain.ValueObjects.Users;

public record UserId
{
    public string Value { get; }

    private UserId(string value) => Value = value;

    private const int LimitLength = 20;

    public static UserId CreateWithValidation(string? value)
    {
        if (value is not { Length: > 0 })
            throw new ValidationErrorException("ユーザーIDを入力してください。");
        if (value.Length > LimitLength)
            throw new ValidationErrorException($"ユーザーIDは{LimitLength}文字以内で入力してください。");

        if (!Regex.IsMatch(value, @"^[a-zA-Z0-9_-]*$"))
            throw new ValidationErrorException(
                "半角英数字、ハイフン、アンダースコア以外の文字は使用できません。"
            );

        return new(value);
    }

    public static UserId Reconstruct(string? value) => value is { } ? new(value) : null!;

    public override string ToString() => Value;

    public string ToLower() => Value.ToLower();
}
