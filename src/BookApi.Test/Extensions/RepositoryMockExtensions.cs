using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.Abstractions.Interfaces;
using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Infrastructure;
using BookApi.Infrastructure.Abstractions.DataModels;
using BookApi.Infrastructure.DataModels;
using BookApi.Infrastructure.Services.Repositories;
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
            .Setup(m => m.FindAsync(It.IsAny<IActor>(), It.IsAny<ItemId>()))
            .Returns(async (IActor actor, ItemId itemId) =>
            {
                // 先にデータを取得
                var repository = repositoryBuilder();
                var entityBeforeUpdated = await repository.FindAsync(actor, itemId);

                // 取得後にエンティティを返却するまでの間にデータが更新されたものとする
                // ⇒リポジトリが返却するバージョンと、実データのバージョンの間にズレがある状況を作る
                using var anotherDbContext = dbContextBuilder();
                var data = anotherDbContext.Set<TDataModel>().AsTracking().Single(x => x.Id == itemId.Value);
                data.VersionId++;
                anotherDbContext.SaveChanges();  // バージョンを更新して保存

                return entityBeforeUpdated;  // 返却されるのは更新前の古いデータ
            });

        return repositoryMock;
    }

    public static Mock<BookRepository> SetupBookRepositoryForConcurrencyTest(
        this Mock<BookRepository> repositoryMock,
        Func<BookRepository> repositoryBuilder,
        Func<BookDbContext> dbContextBuilder
    )
    {
        repositoryMock.CallBase.Should().BeTrue();

        repositoryMock
            .Setup(m => m.FindByISBNAsync(It.IsAny<IActor>(), It.IsAny<ISBNCode>()))
            .Returns(async (IActor actor, ISBNCode isbn) =>
            {
                var repository = repositoryBuilder();
                var entityBeforeUpdated = await repository.FindByISBNAsync(actor, isbn);

                using var anotherDbContext = dbContextBuilder();
                var data = anotherDbContext.Set<BookDataModel>().AsTracking().Single(x => x.ISBN == isbn.Value);
                data.VersionId++;
                anotherDbContext.SaveChanges();

                return entityBeforeUpdated;
            });

        return repositoryMock;
    }
}
