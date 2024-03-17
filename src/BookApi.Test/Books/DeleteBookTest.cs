using BookApi.Infrastructure.DataModels;
using BookApi.Infrastructure.DataModels.Intermediates;
using BookApi.UseCase.Books;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Test.Books;

public class DeleteBookTest : BookUseCaseTestBase
{
    [Fact]
    public async Task 正常系_書籍を削除()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");
        DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, DefaultISBN, "ぼっち・ざ・ろっく！(1)", [1], 1
        );
        var actor = UserFixture.Admin;

        // Act
        await Mediator.Send(new DeleteBook.Command(actor, DefaultISBN));

        // Assert
        DbContext.AssertNotExistData<BookDataModel>();
    }

    [Fact]
    public async Task 正常系_書籍に結びついた著者と出版社は削除しない()
    {
        // Arrange
        var bocchi = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var nijika = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "伊地知虹夏");
        var ryou = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "山田リョウ");
        var ikuyo = DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "喜多郁代");
        var publisher = DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var book1 = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, DefaultISBN, "A book to delete", [1, 2, 3, 4], 1
        );
        var book2 = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-1", "Another book", [2, 3, 4], 1
        );

        var actor = UserFixture.Admin;

        // Act
        await Mediator.Send(new DeleteBook.Command(actor, DefaultISBN));

        // Assert
        DbContext.AssertNotExistData<BookDataModel>(x => x.Id == 1);
        DbContext.AssertNotExistData<BookAuthorDataModel>(x => x.BookId == 1);

        // 出版社、削除された本の他の著者、他の書籍には影響しない
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
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");
        DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, DefaultISBN, "ぼっち・ざ・ろっく！(1)", [1], 1
        );
        var actor = UserFixture.Admin2;  // 別の管理者ユーザー

        // Act
        await Mediator.Send(new DeleteBook.Command(actor, DefaultISBN));

        // Assert
        DbContext.AssertNotExistData<BookDataModel>();
    }

    [Fact]
    public async Task 異常系_一般ユーザーは削除できない()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");
        var origin = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, DefaultISBN, "ぼっち・ざ・ろっく！(1)", [1], 1
        );
        var actor = UserFixture.User1;  // 一般ユーザー

        // Act
        var act = () => Mediator.Send(new DeleteBook.Command(actor, DefaultISBN));

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        AssertBookData(origin, [1]);
    }

    [Fact]
    public async Task 異常系_対象の書籍が見つからない()
    {
        var actor = UserFixture.Admin;
        string notExistentISBN = "123-4-56-789012-3";

        // Act
        var act = () => Mediator.Send(new DeleteBook.Command(actor, notExistentISBN));

        // Assert
        await act.Should().ThrowAsync<ItemNotFoundException>();
    }

    [Fact]
    public async Task 異常系_楽観的排他制御による編集ロック()
    {
        // Arrange mock services
        var serviceProviderWithLocalMock = BuildServiceProviderForConcurrencyTest();

        // Arrange data
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");
        DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, DefaultISBN, "ぼっち・ざ・ろっく！(1)", [1], 1
        );
        var actor = UserFixture.Admin;

        // Act
        var act = () =>
           serviceProviderWithLocalMock
               .GetRequiredService<ISender>()
               .Send(new DeleteBook.Command(actor, DefaultISBN));

        // Assert
        await act.Should().ThrowAsync<ConcurrencyException>();
        var actual = DbContext.Books.Single(x => x.ISBN == DefaultISBN);
        actual.VersionId.Should().Be(1);
    }
}
