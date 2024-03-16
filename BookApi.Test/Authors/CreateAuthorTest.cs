using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Commands;
using BookApi.Infrastructure.DataModels;
using BookApi.UseCase.Authors;

namespace BookApi.Test.Authors;

public class CreateAuthorTest : UseCaseTestBase
{
    private void AssertNotCreatedAuthor(int newItemId = 1)
    {
        bool isExistedAuthor = DbContext.Authors.Any(x => x.Id == newItemId);
        isExistedAuthor.Should().BeFalse();
    }

    private static AuthorDataModel GetExpectedDataModel(int id, IActor actor, AuthorCommandDTO command)
        => new()
        {
            Id = id,
            Name = command.Name,
            CreatedAt = DateTimeFixture.UtcNow,
            CreatedById = actor.UserId,
            CreatedByName = actor.UserName
        };

    [Fact]
    public async Task 正常系_著者を登録()
    {
        // Arrange
        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO("テスト太郎");
        var expected = GetExpectedDataModel(1, actor, command);

        // Act
        var result = await Mediator.Send(new CreateAuthor.Command(actor, command));

        // Assert
        var actual = DbContext.Authors.Single(x => x.Id == result.ItemId);
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 正常系_著者名が30文字の境界値で登録できる()
    {
        // Arrange
        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO(StringFixture.GetAsciiDummyString(30));
        var expected = GetExpectedDataModel(1, actor, command);

        // Act
        var result = await Mediator.Send(new CreateAuthor.Command(actor, command));

        // Assert
        var actual = DbContext.Authors.Single(x => x.Id == result.ItemId);
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 異常系_一般ユーザーは登録できない()
    {
        // Arrange
        var actor = UserFixture.User1;
        var command = new AuthorCommandDTO("テスト太郎");

        // Act
        var act = () => Mediator.Send(new CreateAuthor.Command(actor, command));

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        AssertNotCreatedAuthor();
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
        AssertNotCreatedAuthor();
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
        AssertNotCreatedAuthor();
    }

    [Fact]
    public async Task 異常系_既に登録されている名前の著者は登録できない()
    {
        // Arrange
        // 登録済みのデータを用意
        var fixture = new AuthorDataModel
        {
            Id = 1,
            Name = "テスト太郎",
            CreatedAt = DateTimeFixture.UtcNow,
            CreatedById = UserFixture.Admin.UserId,
            CreatedByName = UserFixture.Admin.UserName
        };
        DbContext.Add(fixture);
        DbContext.SaveChanges();

        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO("テスト太郎");

        // Act
        var act = () => Mediator.Send(new CreateAuthor.Command(actor, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("既に同じ名前の著者が存在します。");
        AssertNotCreatedAuthor(2);
    }
}
