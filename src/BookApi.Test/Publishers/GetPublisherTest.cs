using BookApi.Domain.DTOs.Responses;
using BookApi.UseCase.Publishers;

namespace BookApi.Test.Publishers;

public class GetPublisherTest : PublisherUseCaseTestBase
{
    [Fact]
    public async Task 正常系_出版社の詳細を取得()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var book = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-0", "ぼっち・ざ・ろっく！", [1], 1
        );

        var expected = new PublisherResponseDTO(
            1, "スターリー出版",
            [new("000-0-00-000000-0", "ぼっち・ざ・ろっく！", book.PublishedAt, [1], 1)]
        ) with
        {
            RelatedAuthors = [new(1, "後藤ひとり")]
        };

        var actor = UserFixture.Admin;

        // Act
        var result = await Mediator.Send(new GetPublisher.Query(actor, 1));

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 正常系_関連する書籍と著者名は登録順で列挙()
    {
        // Arrange
        // ISBNや名前欄に関わらず、書籍と著者名は登録順で列挙する
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "Bさん");
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "Aさん");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var book1 = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-1", "いいい", [2, 1], 1
        );
        var book2 = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-0", "あああ", [1], 1
        );

        var actor = UserFixture.Admin;

        // Act
        var result = await Mediator.Send(new GetPublisher.Query(actor, 1));

        // Assert
        var expected = new PublisherResponseDTO(
            1, "スターリー出版",
            [
                new("000-0-00-000000-1", "いいい", book1.PublishedAt, [2, 1], 1),
                new("000-0-00-000000-0", "あああ", book2.PublishedAt, [1], 1),
            ]
        ) with
        {
            RelatedAuthors = [new(1, "Bさん"), new(2, "Aさん")]
        };

        result.Should().BeEquivalentTo(expected);
        result.Books.Should().SatisfyRespectively(
            first => first.Should().BeEquivalentTo(expected.Books.ToArray()[0]),
            second => second.Should().BeEquivalentTo(expected.Books.ToArray()[1])
        );
        result.RelatedAuthors.Should().SatisfyRespectively(
            first => first.Should().BeEquivalentTo(expected.RelatedAuthors.ToArray()[0]),
            second => second.Should().BeEquivalentTo(expected.RelatedAuthors.ToArray()[1])
        );
    }

    [Fact]
    public async Task 関連する著者名は重複させない()
    {
        // Arrange
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "伊地知虹夏");
        var book1 = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-1", "一冊目", [1, 2], 1
        );
        var book2 = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-2", "二冊目", [1], 1
        );
        var book3 = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, "000-0-00-000000-3", "三冊目", [2], 1
        );

        var actor = UserFixture.Admin;

        // Act
        var result = await Mediator.Send(new GetPublisher.Query(actor, 1));

        // Assert
        var expected = new PublisherResponseDTO(
            1, "スターリー出版",
            [
                new("000-0-00-000000-1", "一冊目", book1.PublishedAt, [1, 2], 1),
                new("000-0-00-000000-2", "二冊目", book1.PublishedAt, [1], 1),
                new("000-0-00-000000-3", "三冊目", book2.PublishedAt, [2], 1),
            ]
        ) with
        {
            RelatedAuthors = [new(1, "後藤ひとり"), new(2, "伊地知虹夏")]
        };
        result.Should().BeEquivalentTo(expected);
        result.Books.Should().SatisfyRespectively(
            first => first.Should().BeEquivalentTo(expected.Books.ToArray()[0]),
            second => second.Should().BeEquivalentTo(expected.Books.ToArray()[1]),
            third => third.Should().BeEquivalentTo(expected.Books.ToArray()[2])
        );
        result.RelatedAuthors.Should().SatisfyRespectively(
            first => first.Should().BeEquivalentTo(expected.RelatedAuthors.ToArray()[0]),
            second => second.Should().BeEquivalentTo(expected.RelatedAuthors.ToArray()[1])
        );
    }

    [Fact]
    public async Task 正常系_一般ユーザーも閲覧可能()
    {
        // Arrange
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var expected = new PublisherResponseDTO(1, "スターリー出版", []) with { RelatedAuthors = [] };
        var actor = UserFixture.User1;

        // Act
        var result = await Mediator.Send(new GetPublisher.Query(actor, 1));

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 異常系_対象の出版社が見つからない()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
        var actor = UserFixture.Admin;

        // Act
        var act = () => Mediator.Send(new GetPublisher.Query(actor, 2));

        // Assert
        await act.Should().ThrowAsync<ItemNotFoundException>();
    }
}
