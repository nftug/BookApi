using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.Abstractions.ValueObjects;

namespace BookApi.Domain.Abstractions.Interfaces;

public interface IRepositoryBase<T>
    where T : IAggregateEntity<T>
{
    Task<T?> FindAsync(IActor actor, int itemId);
    Task<bool> AnyAsync(IActor actor, int itemId);
    Task SaveAsync(IActor actor, T entity);
    Task DeleteAsync(IActorPermission permission, T entity);
}
