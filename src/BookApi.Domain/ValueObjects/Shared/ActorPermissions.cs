using BookApi.Domain.Abstractions.ValueObjects;

namespace BookApi.Domain.ValueObjects.Shared;

public class PassThroughPermission(IActor actor) : IActorPermission
{
    public bool CanCreate => true;

    public bool CanUpdate => true;

    public bool CanDelete => true;

    public IActor Actor { get; } = actor;
}

public class AdminOnlyPermission(ActorForPermission actor) : IActorPermission
{
    public bool CanCreate => ((ActorForPermission)Actor).IsAdmin;

    public bool CanUpdate => ((ActorForPermission)Actor).IsAdmin;

    public bool CanDelete => ((ActorForPermission)Actor).IsAdmin;

    public IActor Actor { get; } = actor;
}
