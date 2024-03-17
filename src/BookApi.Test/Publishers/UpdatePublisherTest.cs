using BookApi.Domain.DTOs.Commands;
using BookApi.UseCase.Publishers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Test.Publishers;

public class UpdatePublisherTest : PublisherUseCaseTestBase
{
    public UpdatePublisherTest()
    {
        DateTimeProvider.SetupGet(x => x.UtcNow).Returns(UpdatedAt);
    }

    [Fact]
    public async Task 正常系_出版社名を更新()
    {
        // Arrange
        var origin = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");

        var actor = UserFixture.Admin;
        var command = new PublisherCommandDTO("FOLTパブリッシング");
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, UpdatedAt);

        // Act
        await Mediator.Send(new UpdatePublisher.Command(actor, 1, command));

        // Assert
        DbContext.AssertData(1, expected);
    }

    [Fact]
    public async Task 正常系_出版社名が20文字以内の境界値で更新できる()
    {
        // Arrange
        var origin = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");

        var actor = UserFixture.Admin;
        var command = new PublisherCommandDTO(StringFixture.GetAsciiDummyString(20));
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, UpdatedAt);

        // Act
        await Mediator.Send(new UpdatePublisher.Command(actor, 1, command));

        // Assert
        DbContext.AssertData(1, expected);
    }

    [Fact]
    public async Task 正常系_他の管理者が作成したデータを更新できる()
    {
        // Arrange
        var origin = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");

        var actor = UserFixture.Admin2;
        var command = new PublisherCommandDTO("FOLTパブリッシング");
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, UpdatedAt);

        // Act
        await Mediator.Send(new UpdatePublisher.Command(actor, 1, command));

        // Assert
        DbContext.AssertData(1, expected);
    }

    [Fact]
    public async Task 正常系_取得時と同じ名前で保存できる()
    {
        // Arrange
        var origin = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");

        var actor = UserFixture.Admin;
        var command = new PublisherCommandDTO("スターリー出版");
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, UpdatedAt);

        // Act
        await Mediator.Send(new UpdatePublisher.Command(actor, 1, command));

        // Assert
        DbContext.AssertData(1, expected);
    }

    [Fact]
    public async Task 異常系_一般ユーザーは更新できない()
    {
        // Arrange
        var origin = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var actor = UserFixture.User1;
        var command = new PublisherCommandDTO("FOLTパブリッシング");

        // Act
        var act = () => Mediator.Send(new UpdatePublisher.Command(actor, 1, command));

        // Arrange
        await act.Should().ThrowAsync<ForbiddenException>();
        DbContext.AssertData(1, origin);
    }

    [Theory]
    [InlineData(""), InlineData(null)]
    public async Task 異常系_空の出版社名だと更新できない(string emptyName)
    {
        // Arrange
        var origin = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var actor = UserFixture.Admin;
        var command = new PublisherCommandDTO(emptyName);

        // Act
        var act = () => Mediator.Send(new UpdatePublisher.Command(actor, 1, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("出版社名は空にできません。");
        DbContext.AssertData(1, origin);
    }

    [Fact]
    public async Task 異常系_出版社名が21文字以上だと更新できない()
    {
        // Arrange
        var origin = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var actor = UserFixture.Admin;
        var command = new PublisherCommandDTO(StringFixture.GetAsciiDummyString(21));

        // Act
        var act = () => Mediator.Send(new UpdatePublisher.Command(actor, 1, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("出版社名は20文字以内で入力してください。");
        DbContext.AssertData(1, origin);
    }

    [Fact]
    public async Task 異常系_対象の出版社が見つからない()
    {
        // Arrange
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var actor = UserFixture.Admin;
        var command = new PublisherCommandDTO("FOLTパブリッシング");
        int nonExistentId = 2;

        // Act
        var act = () => Mediator.Send(new UpdatePublisher.Command(actor, nonExistentId, command));

        // Assert
        await act.Should().ThrowAsync<ItemNotFoundException>();
    }

    [Fact]
    public async Task 異常系_既に登録されている名前の出版社名は指定できない()
    {
        // Arrange
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var origin = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "FOLTパブリッシング");   // 更新対象

        var actor = UserFixture.Admin;
        var command = new PublisherCommandDTO("スターリー出版");
        int publisherId = 2;

        // Act
        var act = () => Mediator.Send(new UpdatePublisher.Command(actor, publisherId, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("既に同じ名前の出版社が存在します。");
        DbContext.AssertData(publisherId, origin);
    }

    [Fact]
    public async Task 異常系_楽観的排他制御による編集ロック()
    {
        // Arrange mock services
        var serviceProviderWithLocalMock = BuildServiceProviderForConcurrencyTest();

        // Arrange data
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var actor = UserFixture.Admin;
        var command = new PublisherCommandDTO("FOLTパブリッシング");

        // Act
        var act = () =>
            serviceProviderWithLocalMock
                .GetRequiredService<ISender>()
                .Send(new UpdatePublisher.Command(actor, 1, command));

        // Assert
        await act.Should().ThrowAsync<ConcurrencyException>();
        var actual = DbContext.Publishers.Single(x => x.Id == 1);
        actual.VersionId.Should().Be(1);
    }
}
