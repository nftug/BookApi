using BookApi.Domain.DTOs.Commands;
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
        var origin = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");

        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO("後藤ふたり");
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, UpdatedAt);

        // Act
        await Mediator.Send(new UpdateAuthor.Command(actor, 1, command));

        // Assert
        DbContext.AssertData(1, expected);
    }

    [Fact]
    public async Task 正常系_著者名が30文字以内の境界値で更新できる()
    {
        // Arrange
        var origin = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");

        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO(StringFixture.GetAsciiDummyString(30));
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, UpdatedAt);

        // Act
        await Mediator.Send(new UpdateAuthor.Command(actor, 1, command));

        // Assert
        DbContext.AssertData(1, expected);
    }

    [Fact]
    public async Task 正常系_他の管理者が作成したデータを更新できる()
    {
        // Arrange
        var origin = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");

        var actor = UserFixture.Admin2;
        var command = new AuthorCommandDTO("後藤ふたり");
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, UpdatedAt);

        // Act
        await Mediator.Send(new UpdateAuthor.Command(actor, 1, command));

        // Assert
        DbContext.AssertData(1, expected);
    }

    [Fact]
    public async Task 異常系_一般ユーザーは更新できない()
    {
        // Arrange
        var origin = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var actor = UserFixture.User1;
        var command = new AuthorCommandDTO("後藤ふたり");

        // Act
        var act = () => Mediator.Send(new UpdateAuthor.Command(actor, 1, command));

        // Arrange
        await act.Should().ThrowAsync<ForbiddenException>();
        DbContext.AssertData(1, origin);
    }

    [Theory]
    [InlineData(""), InlineData(null)]
    public async Task 異常系_空の著者名だと更新できない(string emptyName)
    {
        // Arrange
        var origin = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO(emptyName);

        // Act
        var act = () => Mediator.Send(new UpdateAuthor.Command(actor, 1, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("著者名は空にできません。");
        DbContext.AssertData(1, origin);
    }

    [Fact]
    public async Task 異常系_著者名が31文字以上だと更新できない()
    {
        // Arrange
        var origin = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO(StringFixture.GetAsciiDummyString(31));

        // Act
        var act = () => Mediator.Send(new UpdateAuthor.Command(actor, 1, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("著者名は30文字以内で入力してください。");
        DbContext.AssertData(1, origin);
    }

    [Fact]
    public async Task 異常系_対象の著者が見つからない()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
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
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var origin = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ふたり");   // 更新対象

        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO("後藤ひとり");
        int authorId = 2;

        // Act
        var act = () => Mediator.Send(new UpdateAuthor.Command(actor, authorId, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("既に同じ名前の著者が存在します。");
        DbContext.AssertData(authorId, origin);
    }

    [Fact]
    public async Task 異常系_楽観的排他制御による編集ロック()
    {
        // Arrange mock services
        var serviceProviderWithLocalMock = BuildServiceProviderForConcurrencyTest();

        // Arrange data
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
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
