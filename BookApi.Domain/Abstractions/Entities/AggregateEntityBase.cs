namespace BookApi.Domain.Abstractions.Entities;

public abstract class AggregateEntityBase<T> : EntityBase<T>, IAggregateEntity<T>
    where T : AggregateEntityBase<T>
{
    public int VersionId { get; private set; }

    // インフラ層からの再構築に使用
    protected AggregateEntityBase(
        int id,
        DateTime createdAt, DateTime? updatedAt,
        int createdById, string createdByName,
        int? updatedById, string? updatedByName,
        int versionId
    ) : base(id, createdAt, updatedAt, createdById, createdByName, updatedById, updatedByName)
    {
        VersionId = versionId;
    }

    // エンティティの新規作成用
    protected AggregateEntityBase() { }

    public void IncreaseVersionIdFromRepository() => VersionId++;
}
