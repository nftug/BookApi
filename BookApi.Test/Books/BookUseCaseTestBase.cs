using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.Entities;
using BookApi.Domain.Interfaces;
using BookApi.Infrastructure;
using BookApi.Infrastructure.DataModels;
using BookApi.Infrastructure.DataModels.Intermediates;
using BookApi.Infrastructure.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Test.Books;

public abstract class BookUseCaseTestBase : UseCaseTestBase
{
    protected static readonly DateTime CreatedAt = new(2024, 3, 16, 20, 0, 0);
    protected static readonly DateTime UpdatedAt = new(2024, 3, 17, 20, 0, 0);

    protected static BookDataModel GetExpectedDataAfterCreation(
        IActor createdBy, BookCommandDTO command, string isbnCodeFormatted, DateTime createdAt, int itemId = 1
    )
        => new()
        {
            Id = itemId,
            ISBN = isbnCodeFormatted,
            Title = command.Title,
            PublishedAt = command.PublishedAt,
            PublisherId = command.PublisherId,
            CreatedAt = createdAt,
            CreatedById = createdBy.UserId,
            CreatedByName = createdBy.UserName
        };

    protected static BookDataModel GetExpectedDataAfterUpdate(
        BookDataModel originData,
        IActor updatedBy,
        BookCommandDTO command,
        string isbnCodeFormatted,
        DateTime updatedAt
    )
        => new()
        {
            Id = originData.Id,
            ISBN = isbnCodeFormatted,
            Title = command.Title,
            PublishedAt = command.PublishedAt,
            PublisherId = command.PublisherId,
            CreatedAt = originData.CreatedAt,
            CreatedById = originData.CreatedById,
            CreatedByName = originData.CreatedByName,
            UpdatedAt = updatedAt,
            UpdatedById = updatedBy.UserId,
            UpdatedByName = updatedBy.UserName,
            VersionId = originData.VersionId + 1
        };

    protected void VerifyBookAuthor(int bookId, int[] orderedAuthorIds)
    {
        var actualAuthorIds =
            DbContext.Set<BookAuthorDataModel>()
                .Where(x => x.BookId == bookId)
                .OrderBy(x => x.Order)
                .Select(x => x.AuthorId)
                .ToArray();

        actualAuthorIds.Should().Equal(orderedAuthorIds);
    }

    protected IServiceProvider BuildServiceProviderForConcurrencyTest()
    {
        // リポジトリをモックで差し替えるため、呼び出し元のテストメソッド内でのみ有効なService providerを構築する
        // (DBの接続先はクラス内メンバと共有する)
        var dbContext = new BookDbContext(DbContextOptions);
        var BookRepositoryMock =
            new Mock<BookRepository>(dbContext) { CallBase = true }
                .SetupRepositoryForConcurrencyTest<Book, BookDataModel, BookRepository>(
                    repositoryBuilder: () => new BookRepository(dbContext),
                    dbContextBuilder: () => new BookDbContext(DbContextOptions)
                );

        return BuildServiceCollectionBase()
                .AddScoped(_ => dbContext)
                .AddScoped<IBookRepository>(_ => BookRepositoryMock.Object)
                .BuildServiceProvider();
    }
}
