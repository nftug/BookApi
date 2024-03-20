using System.Text;
using BookApi.Domain.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;

namespace BookApi.Infrastructure.Services.Utils;

// Reference: https://qiita.com/Nossa/items/c28ec6a8f36c293c9ae8
public class PasswordService(IConfiguration config) : IPasswordService
{
    private readonly byte[] _salt =
        Encoding.UTF8.GetBytes(
            config.GetValue<string>("HashSalts:PasswordSalt")
            ?? throw new InvalidDataException("HashSalts__PasswordSalt is not configured.")
        );

    public string HashPassword(string rawPassword)
        => Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password: rawPassword,
                salt: _salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

    public bool VerifyPassword(string hashedPassword, string rawPassword)
        => hashedPassword == HashPassword(rawPassword);
}
