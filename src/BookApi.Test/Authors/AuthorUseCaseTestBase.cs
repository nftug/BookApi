using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.Entities;
using BookApi.Domain.Interfaces;
using BookApi.Infrastructure;
using BookApi.Infrastructure.DataModels;
using BookApi.Infrastructure.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Test.Authors;

public abstract class AuthorUseCaseTestBase : UseCaseTestBase
{
    protected static readonly DateTime CreatedAt = new(2024, 3, 16, 20, 0, 0);
    protected static readonly DateTime UpdatedAt = new(2024, 3, 17, 20, 0, 0);

    protected static AuthorDataModel GetExpectedDataAfterCreation(
        IActor createdBy, AuthorCommandDTO command, DateTime createdAt, int itemId = 1
    )
        => new()
        {
            Id = itemId,
            Name = command.Name,
            CreatedAt = createdAt,
            CreatedByUserId = createdBy.UserId
        };

    protected static AuthorDataModel GetExpectedDataAfterUpdate(
        AuthorDataModel originData,
        IActor updatedBy,
        AuthorCommandDTO command,
        DateTime updatedAt
    )
        => new()
        {
            Id = originData.Id,
            Name = command.Name,
            CreatedAt = originData.CreatedAt,
            CreatedByUserId = originData.CreatedByUserId,
            UpdatedAt = updatedAt,
            UpdatedByUserId = updatedBy.UserId,
            VersionId = originData.VersionId + 1
        };

    protected IServiceProvider BuildServiceProviderForConcurrencyTest()
    {
        // リポジトリをモックで差し替えるため、呼び出し元のテストメソッド内でのみ有効なService providerを構築する
        // (DBの接続先はクラス内メンバと共有する)
        var dbContext = new BookDbContext(DbContextOptions);
        var authorRepositoryMock =
            new Mock<AuthorRepository>(dbContext) { CallBase = true }
                .SetupRepositoryForConcurrencyTest<Author, AuthorDataModel, AuthorRepository>(
                    repositoryBuilder: () => new AuthorRepository(dbContext),
                    dbContextBuilder: () => new BookDbContext(DbContextOptions)
                );

        return BuildServiceCollectionBase()
                .AddScoped(_ => dbContext)
                .AddScoped<IAuthorRepository>(_ => authorRepositoryMock.Object)
                .BuildServiceProvider();
    }
}
