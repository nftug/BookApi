namespace BookApi.Domain.Abstractions.ValueObjects;

public interface IActor
{
    public int UserId { get; }
    public string UserName { get; }
}
