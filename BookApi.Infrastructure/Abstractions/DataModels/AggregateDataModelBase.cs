using System.ComponentModel.DataAnnotations;
using BookApi.Domain.Abstractions.Entities;

namespace BookApi.Infrastructure.Abstractions.DataModels;

public abstract class AggregateDataModelBase<TEntity, TSelf>
    : DataModelBase<TEntity, TSelf>, IAggregateDataModel<TEntity, TSelf>
    where TEntity : IAggregateEntity<TEntity>
    where TSelf : DataModelBase<TEntity, TSelf>, new()
{
    [ConcurrencyCheck] public int VersionID { get; set; }

    public override TSelf FromEntity(TEntity entity)
    {
        VersionID = entity.VersionID;
        return base.FromEntity(entity);
    }
}

public interface IAggregateDataModel<TEntity, TSelf> : IDataModel<TEntity, TSelf>
    where TEntity : IAggregateEntity<TEntity>
    where TSelf : IDataModel<TEntity, TSelf>
{
    int VersionID { get; set; }
}
