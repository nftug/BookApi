using BookApi.Domain.DTOs.Commands;
using BookApi.Infrastructure.DataModels;
using BookApi.UseCase.Publishers;

namespace BookApi.Test.Publishers;

public class CreatePublisherTest : PublisherUseCaseTestBase
{
    public CreatePublisherTest()
    {
        DateTimeProvider.SetupGet(x => x.UtcNow).Returns(CreatedAt);
    }

    [Fact]
    public async Task 正常系_出版社を登録()
    {
        // Arrange
        var actor = UserFixture.Admin;
        var command = new PublisherCommandDTO("スターリー出版");
        var expected = GetExpectedDataAfterCreation(actor, command, CreatedAt);

        // Act
        var result = await Mediator.Send(new CreatePublisher.Command(actor, command));

        // Assert
        DbContext.AssertData(result.ItemId, expected);
    }

    [Fact]
    public async Task 正常系_出版社名が20文字の境界値で登録できる()
    {
        // Arrange
        var actor = UserFixture.Admin;
        var command = new PublisherCommandDTO(StringFixture.GetAsciiDummyString(20));
        var expected = GetExpectedDataAfterCreation(actor, command, CreatedAt);

        // Act
        var result = await Mediator.Send(new CreatePublisher.Command(actor, command));

        // Assert
        DbContext.AssertData(result.ItemId, expected);
    }

    [Fact]
    public async Task 異常系_一般ユーザーは登録できない()
    {
        // Arrange
        var actor = UserFixture.User1;
        var command = new PublisherCommandDTO("スターリー出版");

        // Act
        var act = () => Mediator.Send(new CreatePublisher.Command(actor, command));

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        DbContext.AssertNotExistData<PublisherDataModel>();
    }

    [Theory]
    [InlineData(""), InlineData(null)]
    public async Task 異常系_空の出版社名だと登録できない(string emptyName)
    {
        // Arrange
        var actor = UserFixture.Admin;
        var command = new PublisherCommandDTO(emptyName);

        // Act
        var act = () => Mediator.Send(new CreatePublisher.Command(actor, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("出版社名は空にできません。");
        DbContext.AssertNotExistData<PublisherDataModel>();
    }

    [Fact]
    public async Task 異常系_出版社名が21文字以上だと登録できない()
    {
        // Arrange
        var actor = UserFixture.Admin;
        var command = new PublisherCommandDTO(StringFixture.GetAsciiDummyString(21));

        // Act
        var act = () => Mediator.Send(new CreatePublisher.Command(actor, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("出版社名は20文字以内で入力してください。");
        DbContext.AssertNotExistData<PublisherDataModel>();
    }

    [Fact]
    public async Task 異常系_既に登録されている名前の出版社は登録できない()
    {
        // Arrange
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");

        var actor = UserFixture.Admin;
        var command = new PublisherCommandDTO("スターリー出版");

        // Act
        var act = () => Mediator.Send(new CreatePublisher.Command(actor, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("既に同じ名前の出版社が存在します。");
        DbContext.AssertNotExistData<PublisherDataModel>(2);
    }
}
