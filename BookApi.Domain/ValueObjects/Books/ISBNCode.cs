using System.Text.RegularExpressions;
using BookApi.Domain.Exceptions;

namespace BookApi.Domain.ValueObjects.Books;

// Powered by ChatGPT 3.5
// Reference: https://chat.openai.com/share/cb9b62c1-c1d4-4f49-bec5-2ec13a1d5831

/// <summary>
/// 13桁 or 10桁のハイフン区切りでISBNコードを格納する<br />
/// - ISBN-10: 0-00-000000-0<br />
/// - ISBN-13: 000-0-00-000000-0
/// </summary>
/// <value></value>
public record ISBNCode
{
    public string Value { get; }

    private ISBNCode(string value) => Value = value;

    public static ISBNCode Reconstruct(string value) => new(value);

    public static ISBNCode CreateWithValidation(string? value)
    {
        if (value is not { Length: > 0 })
            throw new ValidationErrorException("ISBNコードを入力してください。");

        // ハイフンを削除して数字だけを抽出
        string digitsOnly = Regex.Replace(value, @"[^0-9]", "");

        // ISBNコードの形式に合わせてバリデーションとハイフン区切りの文字列への変換を行う
        string formattedValue = digitsOnly.Length switch
        {
            10 => ValidateAndFormat10DigitISBN(digitsOnly),
            13 => ValidateAndFormat13DigitISBN(digitsOnly),
            _ => throw new ValidationErrorException("無効なISBNコードです。")
        };

        return new(formattedValue);
    }

    private static string ValidateAndFormat10DigitISBN(string digitsOnly)
    {
        if (!Regex.IsMatch(digitsOnly, @"^\d{10}$"))
            throw new ValidationErrorException("無効なISBNコードです。");

        return SplitDigitWithHyphen(digitsOnly, 1, 2, 6, 1);
    }

    private static string ValidateAndFormat13DigitISBN(string digitsOnly)
    {
        if (!Regex.IsMatch(digitsOnly, @"^\d{13}$"))
            throw new ValidationErrorException("無効なISBNコードです。");

        return SplitDigitWithHyphen(digitsOnly, 3, 1, 2, 6, 1);
    }

    private static string SplitDigitWithHyphen(string digitsOnly, params int[] numberOfDigitsPerBlock)
    {
        if (numberOfDigitsPerBlock.Sum() != digitsOnly.Length)
            throw new ArgumentException("Number of digits does not match.");

        var blocks = numberOfDigitsPerBlock
            .Select((x, i) => digitsOnly.Substring(numberOfDigitsPerBlock.Take(i).Sum(), x));

        return string.Join('-', blocks);
    }

    public override string ToString() => Value;
}
