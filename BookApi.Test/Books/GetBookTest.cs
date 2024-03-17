using BookApi.Domain.DTOs.Responses;
using BookApi.UseCase.Books;

namespace BookApi.Test.Books;

public class GetBookTest : BookUseCaseTestBase
{
    [Fact]
    public async Task 正常系_書籍の詳細を取得()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");
        DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt,
            "978-4-83-227072-5",
            "ぼっち・ざ・ろっく！",
            [1], 1,
            new(2019, 2, 27)
        );

        var actor = UserFixture.Admin;

        // Act
        var result = await Mediator.Send(new GetBook.Query(actor, "978-4-83-227072-5"));

        // Assert
        var expected = new BookResponseDTO(
            "978-4-83-227072-5",
            "ぼっち・ざ・ろっく！",
            new(2019, 2, 27),
            [new(1, "はまじあき")],
            new(1, "芳文社")
        );

        result.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("9784832270725"), InlineData("4832270729")]
    public async Task 正常系_ハイフン無しのISBNコードで書籍を指定(string isbnDigitsOnly)
    {
        // Arrange
        string formattedISBN = isbnDigitsOnly switch
        {
            "9784832270725" => "978-4-83-227072-5",
            "4832270729" => "4-83-227072-9",
            { } isbn => isbn
        };
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");
        DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt,
            formattedISBN,
            "ぼっち・ざ・ろっく！",
            [1], 1,
            new(2019, 2, 27)
        );

        var actor = UserFixture.Admin;

        // Act
        var result = await Mediator.Send(new GetBook.Query(actor, isbnDigitsOnly));

        // Assert
        var expected = new BookResponseDTO(
            formattedISBN,
            "ぼっち・ざ・ろっく！",
            new(2019, 2, 27),
            [new(1, "はまじあき")],
            new(1, "芳文社")
        );

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 正常系_著者欄はユーザーが書籍データに登録した順番で列挙する()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");
        DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt,
            "978-4-83-227072-5",
            "ぼっち・ざ・ろっく！",
            [2, 1], 1,
            new(2019, 2, 27)
        );

        var actor = UserFixture.Admin;

        // Act
        var result = await Mediator.Send(new GetBook.Query(actor, "978-4-83-227072-5"));

        // Assert
        var expected = new BookResponseDTO(
            "978-4-83-227072-5",
            "ぼっち・ざ・ろっく！",
            new(2019, 2, 27),
            [new(2, "はまじあき"), new(1, "後藤ひとり")],
            new(1, "芳文社")
        );

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 正常系_一般ユーザーも閲覧可能()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");
        DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt,
            "978-4-83-227072-5",
            "ぼっち・ざ・ろっく！",
            [1], 1,
            new(2019, 2, 27)
        );

        var actor = UserFixture.User1;  // 一般ユーザー

        // Act
        var result = await Mediator.Send(new GetBook.Query(actor, "978-4-83-227072-5"));

        // Assert
        var expected = new BookResponseDTO(
            "978-4-83-227072-5",
            "ぼっち・ざ・ろっく！",
            new(2019, 2, 27),
            [new(1, "はまじあき")],
            new(1, "芳文社")
        );

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task 異常系_対象の書籍が見つからない()
    {
        // Arrange
        var actor = UserFixture.Admin;
        string notExistentISBN = "123-4-56-789012-3";

        // Act
        var act = () => Mediator.Send(new GetBook.Query(actor, notExistentISBN));

        // Assert
        await act.Should().ThrowAsync<ItemNotFoundException>();
    }
}
