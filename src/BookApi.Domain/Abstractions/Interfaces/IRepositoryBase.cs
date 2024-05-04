using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.ValueObjects.Shared;

namespace BookApi.Domain.Abstractions.Interfaces;

public interface IRepositoryBase<T>
    where T : IAggregateEntity<T>
{
    Task<T?> FindAsync(IActor actor, ItemId itemId);
    Task<bool> AnyAsync(IActor actor, ItemId itemId);
    Task SaveAsync(IActor actor, T entity);
    Task DeleteAsync(IActorPermission permission, T entity);
}
