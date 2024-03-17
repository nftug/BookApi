namespace BookApi.Domain.Abstractions.ValueObjects;

public interface IActorPermission
{
    public bool CanCreate { get; }
    public bool CanUpdate { get; }
    public bool CanDelete { get; }
    public IActor Actor { get; }
}
