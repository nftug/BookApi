namespace BookApi.Domain.Abstractions.Entities;

public abstract class AggregateEntityBase<T> : EntityBase<T>, IAggregateEntity<T>
    where T : AggregateEntityBase<T>
{
    public int VersionID { get; private set; }

    // インフラ層からの再構築に使用
    protected AggregateEntityBase(
        int id,
        DateTime createdAt, DateTime? updatedAt,
        int createdByID, string createdByName,
        int? updatedByID, string? updatedByName,
        int versionID
    ) : base(id, createdAt, updatedAt, createdByID, createdByName, updatedByID, updatedByName)
    {
        VersionID = versionID;
    }

    // エンティティの新規作成用
    protected AggregateEntityBase() { }

    public void IncreaseVersionIDFromRepository() => VersionID++;
}
