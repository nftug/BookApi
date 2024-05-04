using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;

namespace BookApi.Domain.Abstractions.Entities;

public abstract class EntityBase<T> : IEntity<T>
    where T : EntityBase<T>
{
    internal ItemId ItemId { get; private set; } = null!;
    internal DateTimeRecord DateTimeRecord { get; private set; } = null!;
    internal ActorRecord ActorRecord { get; private set; } = null!;

    public int Id => ItemId.Value;
    public DateTime CreatedAt => DateTimeRecord.CreatedAt;
    public DateTime? UpdatedAt => DateTimeRecord.UpdatedAt;
    public IActor CreatedBy => ActorRecord.CreatedBy;
    public IActor? UpdatedBy => ActorRecord.UpdatedBy;

    // インフラ層からの再構築に使用
    protected EntityBase(
        int id,
        DateTime createdAt, DateTime? updatedAt,
        string createdByUserId, string? updatedByUserId
    )
    {
        ItemId = ItemId.Reconstruct(id);
        DateTimeRecord = new() { CreatedAt = createdAt, UpdatedAt = updatedAt };
        ActorRecord = ActorRecord.Reconstruct(
            UserId.Reconstruct(createdByUserId), UserId.Reconstruct(updatedByUserId)
        );
    }

    // エンティティの新規作成用
    protected EntityBase() { }

    protected T CreateNew(IActorPermission permission, IDateTimeProvider dateTimeProvider)
    {
        if (!permission.CanCreate) throw new ForbiddenException();

        ItemId = ItemId.NewId();
        DateTimeRecord = new() { CreatedAt = dateTimeProvider.UtcNow };
        ActorRecord = new() { CreatedBy = permission.Actor };
        return (T)this;
    }

    protected void Update(IActorPermission permission, IDateTimeProvider dateTimeProvider)
    {
        if (!permission.CanUpdate) throw new ForbiddenException();

        DateTimeRecord = DateTimeRecord with { UpdatedAt = dateTimeProvider.UtcNow };
        ActorRecord = ActorRecord with { UpdatedBy = permission.Actor };
    }

    public void SetIdFromRepository(int id) => ItemId = ItemId.Reconstruct(id);
}
