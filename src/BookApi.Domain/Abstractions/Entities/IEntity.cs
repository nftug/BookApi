using BookApi.Domain.Abstractions.ValueObjects;

namespace BookApi.Domain.Abstractions.Entities;

public interface IEntity<T> : IEntity
    where T : IEntity<T>
{
}

public interface IEntity
{
    public int Id { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }
    public IActor CreatedBy { get; }
    public IActor? UpdatedBy { get; }

    void SetIdFromRepository(int id);
}