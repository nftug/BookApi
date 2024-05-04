using System.Reflection;
using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.Abstractions.Interfaces;
using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Exceptions;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Infrastructure.Abstractions.DataModels;
using BookApi.Infrastructure.Attributes;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Abstractions.Services;

public abstract class RepositoryBase<TAggregate, TDataModel>(BookDbContext dbContext) : IRepositoryBase<TAggregate>
    where TAggregate : class, IAggregateEntity<TAggregate>
    where TDataModel : class, IAggregateDataModel<TAggregate, TDataModel>, new()
{
    protected readonly BookDbContext DbContext = dbContext;

    protected abstract IQueryable<TDataModel> QueryForRead(IActor actor);

    public virtual async Task<TAggregate?> FindAsync(IActor actor, ItemId itemId)
        => (await QueryForRead(actor).SingleOrDefaultAsync(x => x.Id == itemId.Value))
            ?.ToEntity();

    public virtual async Task<bool> AnyAsync(IActor actor, ItemId itemId)
        => await QueryForRead(actor).AnyAsync(x => x.Id == itemId.Value);

    public virtual async Task SaveAsync(IActor actor, TAggregate entity)
    {
        TDataModel dataModel;

        // IDの有無を確認し、新規作成か更新のどちらを行うか判断する
        if (entity.Id == default)
        {
            dataModel = new TDataModel().FromEntity(entity);
            await DbContext.AddAsync(dataModel);
        }
        else
        {
            dataModel = await GetDataModelForChangeAsync(entity);
            entity.IncreaseVersionIdFromRepository();
            dataModel.FromEntity(entity);

            ClearIntermediateTables();  // 中間テーブルのクリア
        }

        // DBに保存し、付与されたIDをエンティティに反映する
        await SaveChangesAsync();
        entity.SetIdFromRepository(dataModel.Id);

        // 多対多のテーブルがある場合、中間テーブルを再構成する必要がある
        if (dataModel.ReconstructIntermediates(entity))
            await SaveChangesAsync();

        DbContext.ChangeTracker.Clear();    // EF Coreの追跡を解除
    }

    public virtual async Task DeleteAsync(IActorPermission permission, TAggregate entity)
    {
        if (!permission.CanDelete) throw new ForbiddenException();

        var dataModel = await GetDataModelForChangeAsync(entity);

        DbContext.Remove(dataModel);

        // BookDbContextでオーバーライドした処理により、追加のカスケードデリートも行われる
        await SaveChangesAsync();
    }

    protected async Task<TDataModel> GetDataModelForChangeAsync(TAggregate entity)
    {
        var dataModel =
            await DbContext.Set<TDataModel>().AsTracking().SingleAsync(x => x.Id == entity.Id);

        // 楽観ロックのため、エンティティ取得時のVersion IDをEF Coreの追跡に反映する
        DbContext.Entry(dataModel).Property(x => x.VersionId).OriginalValue = entity.VersionId;

        return dataModel;
    }

    protected void ClearIntermediateTables()
    {
        // 保存前に中間テーブルを捜索して削除する
        foreach (var entry in DbContext.ChangeTracker.Entries().ToList())
        {
            var intermediates = entry.Entity.GetType().GetProperties()
                .Where(p => p.GetCustomAttribute<IntermediateAttribute>() != null)
                .Where(p => p.PropertyType.IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                .Select(p => p.GetValue(entry.Entity))
                .OfType<IEnumerable<object>>()
                .SelectMany(e => e);

            foreach (var intermediateEntity in intermediates.ToList())
            {
                DbContext.Entry(intermediateEntity).State = EntityState.Deleted;
            }
        }
    }

    protected async Task SaveChangesAsync()
    {
#if DEBUG
        // デバッグ時にEF Coreによる変更履歴を出力する
        System.Diagnostics.Debug.WriteLine(DbContext.ChangeTracker.DebugView.LongView);
#endif
        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException();
        }
    }
}
