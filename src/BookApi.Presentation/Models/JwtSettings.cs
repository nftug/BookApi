namespace BookApi.Presentation.Models;

public record JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public int ExpireDays { get; set; }
}
