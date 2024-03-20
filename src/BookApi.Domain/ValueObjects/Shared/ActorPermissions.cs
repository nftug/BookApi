using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.Abstractions.ValueObjects;

namespace BookApi.Domain.ValueObjects.Shared;

public class PassThroughPermission(Actor actor) : IActorPermission
{
    public bool CanCreate => true;

    public bool CanUpdate => true;

    public bool CanDelete => true;

    public Actor Actor { get; } = actor;
}

public class AdminOnlyPermission(Actor actor) : IActorPermission
{
    public bool CanCreate => Actor.IsAdmin;

    public bool CanUpdate => Actor.IsAdmin;

    public bool CanDelete => Actor.IsAdmin;

    public Actor Actor { get; } = actor;
}

public class OwnerOnlyPermission(Actor actor, IEntity entity) : IActorPermission
{
    private readonly ActorRecord _actorRecord =
        ActorRecord.Reconstruct(entity.CreatedBy.UserId, entity.UpdatedBy?.UserId);

    public bool CanCreate => true;

    public bool CanUpdate => Actor.IsAdmin || Actor.UserId == _actorRecord.CreatedBy.UserId;

    public bool CanDelete => Actor.IsAdmin || Actor.UserId == _actorRecord.CreatedBy.UserId;

    public Actor Actor { get; } = actor;
}
