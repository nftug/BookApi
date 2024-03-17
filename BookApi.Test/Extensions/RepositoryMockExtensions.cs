using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.Abstractions.Interfaces;
using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Infrastructure;
using BookApi.Infrastructure.Abstractions.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Test.Extensions;

public static class RepositoryMockExtensions
{
    public static Mock<TRepository> SetupRepositoryForConcurrencyTest<TEntity, TDataModel, TRepository>(
        this Mock<TRepository> repositoryMock,
        Func<TRepository> repositoryBuilder,
        Func<BookDbContext> dbContextBuilder
    )
        where TEntity : class, IAggregateEntity<TEntity>
        where TDataModel : class, IAggregateDataModel<TEntity, TDataModel>, new()
        where TRepository : class, IRepositoryBase<TEntity>
    {
        // MockのCallBaseは有効にした状態 (FindAsyncのみをオーバーライドするため)
        repositoryMock.CallBase.Should().BeTrue();

        repositoryMock
            .Setup(m => m.FindAsync(It.IsAny<IActor>(), It.IsAny<int>()))
            .Returns(async (IActor actor, int id) =>
            {
                // 先にデータを取得
                var repository = repositoryBuilder();
                var entityBeforeUpdated = await repository.FindAsync(actor, id);

                // 取得後にエンティティを返却するまでの間にデータが更新されたものとする
                // ⇒リポジトリが返却するバージョンと、実データのバージョンの間にズレがある状況を作る
                using var anotherDbContext = dbContextBuilder();
                var data = anotherDbContext.Set<TDataModel>().AsTracking().Single(x => x.Id == id);
                data.VersionId++;
                anotherDbContext.SaveChanges();  // バージョンを更新して保存

                return entityBeforeUpdated;  // 返却されるのは更新前の古いデータ
            });

        return repositoryMock;
    }
}
