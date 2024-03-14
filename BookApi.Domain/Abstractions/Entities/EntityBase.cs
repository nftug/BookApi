using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Exceptions;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Abstractions.Entities;

public abstract class EntityBase<T> : IEntity<T>
    where T : EntityBase<T>
{
    public int ID { get; private set; }
    public DateTimeRecord DateTimeRecord { get; private set; } = null!;
    public ActorRecord ActorRecord { get; private set; } = null!;

    public DateTime CreatedAt => DateTimeRecord.CreatedAt;
    public DateTime? UpdatedAt => DateTimeRecord.UpdatedAt;
    public IActor CreatedBy => ActorRecord.CreatedBy;
    public IActor? UpdatedBy => ActorRecord.UpdatedBy;

    // インフラ層からの再構築に使用
    protected EntityBase(
        int id,
        DateTime createdAt, DateTime? updatedAt,
        int createdByID, string createdByName,
        int? updatedByID, string? updatedByName
    )
    {
        ID = id;
        DateTimeRecord = new() { CreatedAt = createdAt, UpdatedAt = updatedAt };
        ActorRecord = ActorRecord.Reconstruct(createdByID, createdByName, updatedByID, updatedByName);
    }

    // エンティティの新規作成用
    protected EntityBase() { }

    protected T CreateNew(IActorPermission permission)
    {
        if (!permission.CanCreate) throw new ForbiddenException();

        DateTimeRecord = new() { CreatedAt = DateTime.UtcNow };
        ActorRecord = new() { CreatedBy = permission.Actor };
        return (T)this;
    }

    protected void Update(IActorPermission permission)
    {
        if (!permission.CanUpdate) throw new ForbiddenException();

        DateTimeRecord = DateTimeRecord with { UpdatedAt = DateTime.UtcNow };
        ActorRecord = ActorRecord with { UpdatedBy = permission.Actor };
    }

    public void SetIDFromRepository(int id) => ID = id;
}
