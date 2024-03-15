namespace BookApi.Domain.Abstractions.Entities;

public interface IAggregateEntity<T> : IEntity<T>
    where T : IAggregateEntity<T>
{
    int VersionID { get; }

    void IncreaseVersionIDFromRepository();
}
