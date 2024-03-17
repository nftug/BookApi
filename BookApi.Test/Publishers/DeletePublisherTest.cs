using BookApi.Infrastructure.DataModels;
using BookApi.Infrastructure.DataModels.Intermediates;
using BookApi.UseCase.Publishers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Test.Publishers;

public class DeletePublisherTest : PublisherUseCaseTestBase
{
    [Fact]
    public async Task 正常系_出版社を削除()
    {
        // Arrange
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var actor = UserFixture.Admin;

        // Act
        await Mediator.Send(new DeletePublisher.Command(actor, 1));

        // Assert
        DbContext.AssertNotExistData<PublisherDataModel>(1);
    }

    [Fact]
    public async Task 正常系_出版社に関連付けられた書籍を削除()
    {
        // Arrange
        var bocchi = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var nijika = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "伊地知虹夏");
        var ryou = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "山田リョウ");
        var ikuyo = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "喜多郁代");
        var publisher = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var publisher2 = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "FOLTパブリッシング");
        var book1 = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-0", "A book to delete", [1, 2, 3, 4], 1
        );
        var book2 = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-1", "Another book", [1, 2, 3, 4], 2
        );

        var actor = UserFixture.Admin;

        // Act
        await Mediator.Send(new DeletePublisher.Command(actor, 1));

        // Assert
        // 出版社とそれに関連付けられた書籍が削除
        DbContext.AssertNotExistData<PublisherDataModel>(1);
        DbContext.AssertNotExistData<BookDataModel>(1);

        // 中間テーブルも削除
        DbContext.AssertNotExistData<BookAuthorDataModel>(x => x.BookId == 1);

        // 出版社、削除された本の著者、関係のない書籍には影響しない
        DbContext.AssertData(1, bocchi);
        DbContext.AssertData(2, nijika);
        DbContext.AssertData(3, ryou);
        DbContext.AssertData(4, ikuyo);
        DbContext.AssertData(2, publisher2);
        DbContext.AssertData(2, book2);
        DbContext.Set<BookAuthorDataModel>()
            .Where(x => x.BookId == 2)
            .Select(x => x.AuthorId)
            .Should()
            .BeEquivalentTo([1, 2, 3, 4]);
    }

    [Fact]
    public async Task 正常系_他の管理者が作成したデータを削除できる()
    {
        // Arrange
        var origin = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var publisher = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var book = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-0", "A book to delete", [1], 1
        );

        var actor = UserFixture.Admin2;

        // Act
        await Mediator.Send(new DeletePublisher.Command(actor, 1));

        // Assert
        // 出版社とそれに関連付けられた書籍が削除
        DbContext.AssertNotExistData<PublisherDataModel>(1);
        DbContext.AssertNotExistData<BookDataModel>(1);

        // 中間テーブルも削除
        DbContext.AssertNotExistData<BookAuthorDataModel>(x => x.BookId == book.Id);
    }

    [Fact]
    public async Task 異常系_一般ユーザーは削除できない()
    {
        // Arrange
        var origin = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var actor = UserFixture.User1;

        // Act
        var act = () => Mediator.Send(new DeletePublisher.Command(actor, 1));

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        DbContext.AssertData(origin.Id, origin);
    }

    [Fact]
    public async Task 異常系_対象の出版社が見つからない()
    {
        // Arrange
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var actor = UserFixture.Admin;

        // Act
        var act = () => Mediator.Send(new DeletePublisher.Command(actor, 2));

        // Assert
        await act.Should().ThrowAsync<ItemNotFoundException>();
    }

    [Fact]
    public async Task 異常系_楽観的排他制御による編集ロック()
    {
        // Arrange mock services
        var serviceProviderWithLocalMock = BuildServiceProviderForConcurrencyTest();

        // Arrange data
        var origin = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var actor = UserFixture.Admin;

        // Act
        var act = () =>
            serviceProviderWithLocalMock
                .GetRequiredService<ISender>()
                .Send(new DeletePublisher.Command(actor, 1));

        // Assert
        await act.Should().ThrowAsync<ConcurrencyException>();
        var actual = DbContext.Publishers.Single(x => x.Id == 1);
        actual.VersionId.Should().Be(1);
    }
}
