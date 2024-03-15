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
    [Key] public int ID { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CreatedByID { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public int? UpdatedByID { get; set; }
    public string? UpdatedByName { get; set; }

    // レイヤー間の詰替え用
    public abstract TEntity ToEntity();

    public virtual TSelf FromEntity(TEntity entity)
    {
        CreatedAt = entity.CreatedAt;
        UpdatedAt = entity.UpdatedAt;
        CreatedByID = entity.CreatedBy.UserId;
        CreatedByName = entity.CreatedBy.UserName;
        UpdatedByID = entity.UpdatedBy?.UserId;
        UpdatedByName = entity.UpdatedBy?.UserName;
        OnTransferFromEntity(entity);

        return (TSelf)this;
    }

    protected virtual void OnTransferFromEntity(TEntity entity) { }

    /// <summary>
    /// 中間テーブルの作成などに使用する後処理<br />
    /// 処理後にSaveChangesを実行する場合はtrueを返す
    /// </summary>
    public virtual bool OnTransferAfterSave(TEntity entity) => false;

    // 中間テーブル初期化の処理
    public virtual void ClearIntermediates(BookDbContext dbContext) { }

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

public interface IDataModel<TEntity, TSelf>
    where TEntity : IEntity<TEntity>
    where TSelf : IDataModel<TEntity, TSelf>
{
    int ID { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
    int CreatedByID { get; set; }
    string CreatedByName { get; set; }
    int? UpdatedByID { get; set; }
    string? UpdatedByName { get; set; }

    TEntity ToEntity();
    TSelf FromEntity(TEntity entity);
    void ClearIntermediates(BookDbContext dbContext);
    bool OnTransferAfterSave(TEntity entity);
}
