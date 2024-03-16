using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.Entities;
using BookApi.Domain.Interfaces;
using BookApi.Infrastructure;
using BookApi.Infrastructure.DataModels;
using BookApi.Infrastructure.Services.Repositories;
using BookApi.UseCase.Authors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Test.Authors;

public class UpdateAuthorTest : AuthorUseCaseTestBase
{
    public UpdateAuthorTest()
    {
        DateTimeProvider.SetupGet(x => x.UtcNow).Returns(UpdatedAt);
    }

    [Fact]
    public async Task 正常系_著者名を更新()
    {
        // Arrange
        var origin = AddDataToDatabase(UserFixture.Admin, "後藤ひとり", CreatedAt);

        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO("後藤ふたり");
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, UpdatedAt);

        // Act
        await Mediator.Send(new UpdateAuthor.Command(actor, 1, command));

        // Assert
        AssertData(1, expected);
    }

    [Fact]
    public async Task 正常系_著者名が30文字以内の境界値で更新できる()
    {
        // Arrange
        var origin = AddDataToDatabase(UserFixture.Admin, "後藤ひとり", CreatedAt);

        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO(StringFixture.GetAsciiDummyString(30));
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, UpdatedAt);

        // Act
        await Mediator.Send(new UpdateAuthor.Command(actor, 1, command));

        // Assert
        AssertData(1, expected);
    }

    [Fact]
    public async Task 異常系_一般ユーザーは更新できない()
    {
        // Arrange
        var origin = AddDataToDatabase(UserFixture.Admin, "後藤ひとり", CreatedAt);
        var actor = UserFixture.User1;
        var command = new AuthorCommandDTO("後藤ふたり");

        // Act
        var act = () => Mediator.Send(new UpdateAuthor.Command(actor, 1, command));

        // Arrange
        await act.Should().ThrowAsync<ForbiddenException>();
        AssertData(1, origin);
    }

    [Theory]
    [InlineData(""), InlineData(null)]
    public async Task 異常系_空の著者名だと更新できない(string emptyName)
    {
        // Arrange
        var origin = AddDataToDatabase(UserFixture.Admin, "後藤ひとり", CreatedAt);
        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO(emptyName);

        // Act
        var act = () => Mediator.Send(new UpdateAuthor.Command(actor, 1, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("著者名は空にできません。");
        AssertData(1, origin);
    }

    [Fact]
    public async Task 異常系_著者名が31文字以上だと更新できない()
    {
        // Arrange
        var origin = AddDataToDatabase(UserFixture.Admin, "後藤ひとり", CreatedAt);
        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO(StringFixture.GetAsciiDummyString(31));

        // Act
        var act = () => Mediator.Send(new UpdateAuthor.Command(actor, 1, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("著者名は30文字以内で入力してください。");
        AssertData(1, origin);
    }

    [Fact]
    public async Task 異常系_著者が見つからない()
    {
        // Arrange
        AddDataToDatabase(UserFixture.Admin, "後藤ひとり", CreatedAt);
        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO("後藤ふたり");
        int nonExistentId = 2;

        // Act
        var act = () => Mediator.Send(new UpdateAuthor.Command(actor, nonExistentId, command));

        // Assert
        await act.Should().ThrowAsync<ItemNotFoundException>();
    }

    [Fact]
    public async Task 異常系_既に登録されている名前の著者名は指定できない()
    {
        // Arrange
        AddDataToDatabase(UserFixture.Admin, "後藤ひとり", CreatedAt);
        var origin = AddDataToDatabase(UserFixture.Admin, "後藤ふたり", CreatedAt);   // 更新対象

        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO("後藤ひとり");
        int authorId = 2;

        // Act
        var act = () => Mediator.Send(new UpdateAuthor.Command(actor, authorId, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("既に同じ名前の著者が存在します。");
        AssertData(authorId, origin);
    }

    [Fact]
    public async Task 異常系_楽観的排他制御による編集ロック()
    {
        // Arrange mock services
        // リポジトリをモックで差し替えるため、このテストメソッド内でのみ有効なService providerを構築する
        // (DBの接続先はクラス内メンバと共有する)
        var dbContext = new BookDbContext(DbContextOptions);
        var authorRepositoryMock =
            new Mock<AuthorRepository>(dbContext) { CallBase = true }
                .SetupRepositoryForConcurrencyTest<Author, AuthorDataModel, AuthorRepository>(
                    repositoryBuilder: () => new AuthorRepository(dbContext),
                    dbContextBuilder: () => new BookDbContext(DbContextOptions)
                );

        var serviceProviderWithLocalMock =
             BuildServiceCollectionBase()
                .AddScoped(_ => dbContext)
                .AddScoped<IAuthorRepository>(_ => authorRepositoryMock.Object)
                .BuildServiceProvider();

        // Arrange data
        AddDataToDatabase(UserFixture.Admin, "後藤ひとり", CreatedAt);
        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO("後藤ふたり");

        // Act
        var act = () =>
            serviceProviderWithLocalMock
                .GetRequiredService<ISender>()
                .Send(new UpdateAuthor.Command(actor, 1, command));

        // Assert
        await act.Should().ThrowAsync<ConcurrencyException>();
        var actual = DbContext.Authors.Single(x => x.Id == 1);
        actual.VersionId.Should().Be(1);
    }
}
