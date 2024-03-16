using BookApi.Domain.DTOs.Commands;
using BookApi.UseCase.Authors;

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
        var origin = ArrangeStoredData(UserFixture.Admin, "テスト太郎", CreatedAt);

        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO("テスト次郎");
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
        var origin = ArrangeStoredData(UserFixture.Admin, "テスト太郎", CreatedAt);

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
        var origin = ArrangeStoredData(UserFixture.Admin, "テスト太郎", CreatedAt);
        var actor = UserFixture.User1;
        var command = new AuthorCommandDTO("テスト次郎");

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
        var origin = ArrangeStoredData(UserFixture.Admin, "テスト太郎", CreatedAt);
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
        var origin = ArrangeStoredData(UserFixture.Admin, "テスト太郎", CreatedAt);
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
        ArrangeStoredData(UserFixture.Admin, "テスト太郎", CreatedAt);
        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO("テスト次郎");
        int nonExistentId = 2;

        // Act
        var act = () => Mediator.Send(new UpdateAuthor.Command(actor, nonExistentId, command));

        // Assert
        await act.Should().ThrowAsync<ItemNotFoundException>();
    }
}
