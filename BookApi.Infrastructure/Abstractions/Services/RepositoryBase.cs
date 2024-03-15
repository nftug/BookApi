using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.Abstractions.Interfaces;
using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.Exceptions;
using BookApi.Infrastructure.Abstractions.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.Abstractions.Services;

public abstract class RepositoryBase<TAggregate, TDataModel>(BookDbContext dbContext) : IRepositoryBase<TAggregate>
    where TAggregate : class, IAggregateEntity<TAggregate>
    where TDataModel : class, IAggregateDataModel<TAggregate, TDataModel>, new()
{
    protected readonly BookDbContext DbContext = dbContext;

    protected abstract IQueryable<TDataModel> QueryForRead(IActor actor);

    protected abstract IQueryable<TDataModel> QueryForSave(IActor actor);

    public virtual async Task<TAggregate?> FindAsync(IActor actor, int itemID)
        => (await QueryForRead(actor).Where(x => x.ID == itemID).SingleOrDefaultAsync())
            ?.ToEntity();

    public virtual async Task<bool> AnyAsync(IActor actor, int itemID)
        => await QueryForRead(actor).AnyAsync(x => x.ID == itemID);

    public virtual async Task SaveAsync(IActor actor, TAggregate entity)
    {
        TDataModel dataModel;

        // IDの有無を確認し、新規作成か更新のどちらを行うか判断する
        if (entity.ID == default)
        {
            dataModel = new TDataModel().FromEntity(entity);
            await DbContext.AddAsync(dataModel);
        }
        else
        {
            dataModel = await QueryForSave(actor).AsTracking().SingleAsync(x => x.ID == entity.ID);

            // 楽観ロックのため、エンティティ取得時のVersion IDをEF Coreの追跡に反映する
            DbContext.Entry(dataModel).Property(x => x.VersionID).OriginalValue = entity.VersionID;

            entity.IncreaseVersionIDFromRepository();
            dataModel.FromEntity(entity);
        }

        // DBに保存し、付与されたIDをエンティティに反映する
        await SaveChangesAsync();
        entity.SetIDFromRepository(dataModel.ID);

        // 保存後に後処理と追加の保存が必要な場合は実行する (中間テーブルの作成など)
        bool shouldDoPostTransfer = dataModel.OnTransferAfterSave(entity);
        //if (shouldDoPostTransfer) await SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(IActorPermission permission, TAggregate entity)
    {
        if (!permission.CanDelete) throw new ForbiddenException();

        var dataModel = new TDataModel { ID = entity.ID, VersionID = entity.VersionID };
        DbContext.Attach(dataModel);
        DbContext.Remove(dataModel);

        await SaveChangesAsync();
    }

    protected async Task SaveChangesAsync()
    {
#if DEBUG
        // デバッグ時にEF Coreによる変更履歴を出力する
        Console.WriteLine(DbContext.ChangeTracker.DebugView.ShortView);
#endif

        await DbContext.SaveChangesAsync();
    }
}
