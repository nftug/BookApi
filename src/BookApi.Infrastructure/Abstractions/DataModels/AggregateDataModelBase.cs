using System.ComponentModel.DataAnnotations;
using BookApi.Domain.Abstractions.Entities;

namespace BookApi.Infrastructure.Abstractions.DataModels;

public abstract class AggregateDataModelBase<TEntity, TSelf>
    : DataModelBase<TEntity, TSelf>, IAggregateDataModel<TEntity, TSelf>
    where TEntity : IAggregateEntity<TEntity>
    where TSelf : DataModelBase<TEntity, TSelf>, new()
{
    [ConcurrencyCheck] public int VersionId { get; set; }

    public override TSelf FromEntity(TEntity entity)
    {
        VersionId = entity.VersionId;
        return base.FromEntity(entity);
    }
}

public interface IAggregateDataModel<TEntity, TSelf> : IDataModel<TEntity, TSelf>, IAggregateDataModel
    where TEntity : IAggregateEntity<TEntity>
    where TSelf : IDataModel<TEntity, TSelf>
{
}

public interface IAggregateDataModel : IDataModel
{
    int VersionId { get; set; }
}
