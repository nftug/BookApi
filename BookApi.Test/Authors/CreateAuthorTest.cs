using BookApi.Domain.DTOs.Commands;
using BookApi.Infrastructure.DataModels;
using BookApi.UseCase.Authors;

namespace BookApi.Test.Authors;

public class CreateAuthorTest : AuthorUseCaseTestBase
{
    public CreateAuthorTest()
    {
        DateTimeProvider.SetupGet(x => x.UtcNow).Returns(CreatedAt);
    }

    [Fact]
    public async Task 正常系_著者を登録()
    {
        // Arrange
        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO("後藤ひとり");
        var expected = GetExpectedDataAfterCreation(actor, command, CreatedAt);

        // Act
        var result = await Mediator.Send(new CreateAuthor.Command(actor, command));

        // Assert
        DbContext.AssertData(result.ItemId, expected);
    }

    [Fact]
    public async Task 正常系_著者名が30文字の境界値で登録できる()
    {
        // Arrange
        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO(StringFixture.GetAsciiDummyString(30));
        var expected = GetExpectedDataAfterCreation(actor, command, CreatedAt);

        // Act
        var result = await Mediator.Send(new CreateAuthor.Command(actor, command));

        // Assert
        DbContext.AssertData(result.ItemId, expected);
    }

    [Fact]
    public async Task 異常系_一般ユーザーは登録できない()
    {
        // Arrange
        var actor = UserFixture.User1;
        var command = new AuthorCommandDTO("後藤ひとり");

        // Act
        var act = () => Mediator.Send(new CreateAuthor.Command(actor, command));

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        DbContext.AssertNotExistData<AuthorDataModel>();
    }

    [Theory]
    [InlineData(""), InlineData(null)]
    public async Task 異常系_空の著者名だと登録できない(string emptyName)
    {
        // Arrange
        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO(emptyName);

        // Act
        var act = () => Mediator.Send(new CreateAuthor.Command(actor, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("著者名は空にできません。");
        DbContext.AssertNotExistData<AuthorDataModel>();
    }

    [Fact]
    public async Task 異常系_著者名が31文字以上だと登録できない()
    {
        // Arrange
        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO(StringFixture.GetAsciiDummyString(31));

        // Act
        var act = () => Mediator.Send(new CreateAuthor.Command(actor, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("著者名は30文字以内で入力してください。");
        DbContext.AssertNotExistData<AuthorDataModel>();
    }

    [Fact]
    public async Task 異常系_既に登録されている名前の著者は登録できない()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");

        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO("後藤ひとり");

        // Act
        var act = () => Mediator.Send(new CreateAuthor.Command(actor, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("既に同じ名前の著者が存在します。");
        DbContext.AssertNotExistData<AuthorDataModel>(2);
    }
}
