namespace BookApi.Domain.Interfaces;

public interface IPasswordService
{
    string HashPassword(string rawPassword);
    bool VerifyPassword(string hashedPassword, string rawPassword);
}
