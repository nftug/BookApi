using BookApi.Domain.DTOs.Responses;
using BookApi.UseCase.Authors;

namespace BookApi.Test.Authors;

public class GetAuthorTest : AuthorUseCaseTestBase
{
    [Fact]
    public async Task 正常系_著者の詳細を取得()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var book = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-0", "ぼっち・ざ・ろっく！", [1], 1
        );

        var expected = new AuthorResponseDTO(
            1, "後藤ひとり",
            [new("000-0-00-000000-0", "ぼっち・ざ・ろっく！", book.PublishedAt, [1], 1)]
        ) with
        {
            RelatedPublishers = [new(1, "スターリー出版")]
        };

        var actor = UserFixture.Admin;

        // Act
        var result = await Mediator.Send(new GetAuthor.Query(actor, 1));

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 正常系_関連する書籍と出版社は登録順で列挙()
    {
        // Arrange
        // ISBNや名前欄に関わらず、書籍と出版社は登録順で列挙する
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "B社");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "A社");
        var book1 = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-1", "いいい", [1], 1
        );
        var book2 = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-0", "あああ", [1], 2
        );

        var expected = new AuthorResponseDTO(
            1, "後藤ひとり",
            [
                new("000-0-00-000000-1", "いいい", book1.PublishedAt, [1], 1),
                new("000-0-00-000000-0", "あああ", book2.PublishedAt, [1], 2),
            ]
        ) with
        {
            RelatedPublishers = [new(1, "B社"), new(2, "A社")]
        };

        var actor = UserFixture.Admin;

        // Act
        var result = await Mediator.Send(new GetAuthor.Query(actor, 1));

        // Assert
        result.Should().BeEquivalentTo(expected);
        result.Books.Should().SatisfyRespectively(
            first => first.Should().BeEquivalentTo(expected.Books.ToArray()[0]),
            second => second.Should().BeEquivalentTo(expected.Books.ToArray()[1])
        );
        result.RelatedPublishers.Should().SatisfyRespectively(
            first => first.Should().BeEquivalentTo(expected.RelatedPublishers.ToArray()[0]),
            second => second.Should().BeEquivalentTo(expected.RelatedPublishers.ToArray()[1])
        );
    }

    [Fact]
    public async Task 正常系_一般ユーザーも閲覧可能()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var expected = new AuthorResponseDTO(1, "後藤ひとり", []) with { RelatedPublishers = [] };
        var actor = UserFixture.User1;

        // Act
        var result = await Mediator.Send(new GetAuthor.Query(actor, 1));

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 異常系_対象の著者が見つからない()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        var actor = UserFixture.Admin;

        // Act
        var act = () => Mediator.Send(new GetAuthor.Query(actor, 2));

        // Assert
        await act.Should().ThrowAsync<ItemNotFoundException>();
    }
}
