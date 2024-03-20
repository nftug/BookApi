namespace BookApi.Infrastructure.Models;

public record InitialUserSettings
{
    public string UserId { get; set; } = "admin";
    public string UserName { get; set; } = "Admin";
    public string Password { get; set; } = "password";
}
