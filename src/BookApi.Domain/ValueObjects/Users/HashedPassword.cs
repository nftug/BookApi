using System.Text.RegularExpressions;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;

namespace BookApi.Domain.ValueObjects.Users;

public record HashedPassword
{
    public string Value { get; }

    private HashedPassword(string hashedPassword) => Value = hashedPassword;

    public static HashedPassword CreateWithValidation(IPasswordService passwordService, string rawPassword)
    {
        if (rawPassword is not { Length: >= 8 })
            throw new ValidationErrorException("パスワードは8文字以上で入力してください。");

        string pattern = @"^[a-zA-Z0-9!@#$%^&*()\-_=+{}[\]|\\;:'""<>,.?/]*$";
        if (!Regex.IsMatch(rawPassword, pattern))
            throw new ValidationErrorException("半角英数字と記号以外の文字は使用できません。");

        return new(passwordService.HashPassword(rawPassword));
    }

    public static HashedPassword Reconstruct(string hashedPassword) => new(hashedPassword);

    public bool VerifyPassword(IPasswordService passwordService, string rawPassword)
        => passwordService.VerifyPassword(Value, rawPassword);

    public HashedPassword ChangePassword(
        IPasswordService passwordService, string oldPasswordRaw, string newPasswordRaw
    )
    {
        if (!VerifyPassword(passwordService, oldPasswordRaw))
            throw new ValidationErrorException("現在のパスワードが違います。");

        return CreateWithValidation(passwordService, newPasswordRaw);
    }
}
