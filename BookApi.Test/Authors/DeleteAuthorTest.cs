using BookApi.Infrastructure.DataModels;
using BookApi.Infrastructure.DataModels.Intermediates;
using BookApi.UseCase.Authors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Test.Authors;

public class DeleteAuthorTest : AuthorUseCaseTestBase
{
    [Fact]
    public async Task 正常系_著者を削除()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var actor = UserFixture.Admin;

        // Act
        await Mediator.Send(new DeleteAuthor.Command(actor, 1));

        // Assert
        DbContext.AssertNotExistData<AuthorDataModel>(1);
    }

    [Fact]
    public async Task 正常系_著者に関連付けられた書籍を削除()
    {
        // Arrange
        var bocchi = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var nijika = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "伊地知虹夏");
        var ryou = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "山田リョウ");
        var ikuyo = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "喜多郁代");
        var publisher = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var book1 = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-0", "A book to delete", [1, 2, 3, 4], 1
        );
        var book2 = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-1", "Another book", [2, 3, 4], 1
        );

        var actor = UserFixture.Admin;

        // Act
        await Mediator.Send(new DeleteAuthor.Command(actor, 1));

        // Assert
        // 著者とそれに関連付けられた書籍が削除
        DbContext.AssertNotExistData<AuthorDataModel>(1);
        DbContext.AssertNotExistData<BookDataModel>(1);

        // 中間テーブルも削除
        DbContext.AssertNotExistData<BookAuthorDataModel>(x => x.BookId == 1);

        // 出版社、削除された本の他の著者、関係のない書籍には影響しない
        DbContext.AssertData(2, nijika);
        DbContext.AssertData(3, ryou);
        DbContext.AssertData(4, ikuyo);
        DbContext.AssertData(1, publisher);
        DbContext.AssertData(2, book2);
        DbContext.Set<BookAuthorDataModel>()
            .Where(x => x.BookId == 2)
            .Select(x => x.AuthorId)
            .Should()
            .BeEquivalentTo([2, 3, 4]);
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
        await Mediator.Send(new DeleteAuthor.Command(actor, 1));

        // Assert
        // 著者とそれに関連付けられた書籍が削除
        DbContext.AssertNotExistData<AuthorDataModel>(1);
        DbContext.AssertNotExistData<BookDataModel>(1);

        // 中間テーブルも削除
        DbContext.AssertNotExistData<BookAuthorDataModel>(x => x.BookId == 1);
    }

    [Fact]
    public async Task 異常系_一般ユーザーは削除できない()
    {
        // Arrange
        var origin = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var actor = UserFixture.User1;

        // Act
        var act = () => Mediator.Send(new DeleteAuthor.Command(actor, 1));

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        DbContext.AssertData(1, origin);
    }

    [Fact]
    public async Task 異常系_対象の著者が見つからない()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var actor = UserFixture.Admin;

        // Act
        var act = () => Mediator.Send(new DeleteAuthor.Command(actor, 2));

        // Assert
        await act.Should().ThrowAsync<ItemNotFoundException>();
    }

    [Fact]
    public async Task 異常系_楽観的排他制御による編集ロック()
    {
        // Arrange mock services
        var serviceProviderWithLocalMock = BuildServiceProviderForConcurrencyTest();

        // Arrange data
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var actor = UserFixture.Admin;

        // Act
        var act = () =>
            serviceProviderWithLocalMock
                .GetRequiredService<ISender>()
                .Send(new DeleteAuthor.Command(actor, 1));

        // Assert
        await act.Should().ThrowAsync<ConcurrencyException>();
        var actual = DbContext.Authors.Single(x => x.Id == 1);
        actual.VersionId.Should().Be(1);
    }
}
