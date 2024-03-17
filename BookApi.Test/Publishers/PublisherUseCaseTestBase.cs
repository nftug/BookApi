using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.Entities;
using BookApi.Domain.Interfaces;
using BookApi.Infrastructure;
using BookApi.Infrastructure.DataModels;
using BookApi.Infrastructure.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Test.Publishers;

public abstract class PublisherUseCaseTestBase : UseCaseTestBase
{
    protected static readonly DateTime CreatedAt = new(2024, 3, 16, 20, 0, 0);
    protected static readonly DateTime UpdatedAt = new(2024, 3, 17, 20, 0, 0);

    protected static PublisherDataModel GetExpectedDataAfterCreation(
        IActor createdBy, PublisherCommandDTO command, DateTime createdAt, int itemId = 1
    )
        => new()
        {
            Id = itemId,
            Name = command.Name,
            CreatedAt = createdAt,
            CreatedById = createdBy.UserId,
            CreatedByName = createdBy.UserName
        };

    protected static PublisherDataModel GetExpectedDataAfterUpdate(
        PublisherDataModel originData,
        IActor updatedBy,
        PublisherCommandDTO command,
        DateTime updatedAt
    )
        => new()
        {
            Id = originData.Id,
            Name = command.Name,
            CreatedAt = originData.CreatedAt,
            CreatedById = originData.CreatedById,
            CreatedByName = originData.CreatedByName,
            UpdatedAt = updatedAt,
            UpdatedById = updatedBy.UserId,
            UpdatedByName = updatedBy.UserName,
            VersionId = originData.VersionId + 1
        };

    protected IServiceProvider BuildServiceProviderForConcurrencyTest()
    {
        // リポジトリをモックで差し替えるため、呼び出し元のテストメソッド内でのみ有効なService providerを構築する
        // (DBの接続先はクラス内メンバと共有する)
        var dbContext = new BookDbContext(DbContextOptions);
        var PublisherRepositoryMock =
            new Mock<PublisherRepository>(dbContext) { CallBase = true }
                .SetupRepositoryForConcurrencyTest<Publisher, PublisherDataModel, PublisherRepository>(
                    repositoryBuilder: () => new PublisherRepository(dbContext),
                    dbContextBuilder: () => new BookDbContext(DbContextOptions)
                );

        return BuildServiceCollectionBase()
                .AddScoped(_ => dbContext)
                .AddScoped<IPublisherRepository>(_ => PublisherRepositoryMock.Object)
                .BuildServiceProvider();
    }
}
