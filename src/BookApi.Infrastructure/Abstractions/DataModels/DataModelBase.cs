using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.Abstractions.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Abstractions.DataModels;

public abstract class DataModelBase<TEntity, TSelf> : IDataModel<TEntity, TSelf>
    where TEntity : IEntity<TEntity>
    where TSelf : DataModelBase<TEntity, TSelf>, new()
{
    [Key] public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? UpdatedByUserId { get; set; }

    // レイヤー間の詰替え用
    public abstract TEntity ToEntity();

    public virtual TSelf FromEntity(TEntity entity)
    {
        CreatedAt = entity.CreatedAt;
        UpdatedAt = entity.UpdatedAt;
        CreatedByUserId = entity.CreatedBy.UserId;
        UpdatedByUserId = entity.UpdatedBy?.UserId;
        OnTransferFromEntity(entity);

        return (TSelf)this;
    }

    protected virtual void OnTransferFromEntity(TEntity entity) { }

    /// <summary>
    /// 中間テーブルの作成などに使用する後処理<br />
    /// 処理後にSaveChangesを実行する場合はtrueを返す
    /// </summary>
    public virtual bool ReconstructIntermediates(TEntity entity) => false;

    // データモデル作成時の処理
    public static void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TSelf>().ToTable(typeof(TEntity).Name);
        new TSelf().OnModelCreating(modelBuilder);
    }

    protected virtual void OnModelCreating(ModelBuilder modelBuilder) { }

    // クエリ発行時の条件
    public static Expression<Func<TSelf, bool>> QueryPredicate(IActor actor)
        => new TSelf().QueryPredicateCore(actor);

    protected virtual Expression<Func<TSelf, bool>> QueryPredicateCore(IActor actor) => x => true;
}

public interface IDataModel<TEntity, TSelf> : IDataModel
    where TEntity : IEntity<TEntity>
    where TSelf : IDataModel<TEntity, TSelf>
{
    TEntity ToEntity();
    TSelf FromEntity(TEntity entity);
    bool ReconstructIntermediates(TEntity entity);
}

public interface IDataModel
{
    int Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
    string CreatedByUserId { get; set; }
    string? UpdatedByUserId { get; set; }
}
