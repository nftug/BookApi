using BookApi.Domain.DTOs.Commands;
using BookApi.Infrastructure.DataModels;
using BookApi.UseCase.Books;

namespace BookApi.Test.Books;

public class CreateBookTest : BookUseCaseTestBase
{
    public CreateBookTest()
    {
        DateTimeProvider.SetupGet(x => x.UtcNow).Returns(CreatedAt);
    }

    [Theory]
    [InlineData("978-4-83-227072-5"), InlineData("4-83-227072-9")]
    public async Task 正常系_書籍を登録(string isbnCode)
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");

        var actor = UserFixture.Admin;
        var command = new BookCommandDTO(isbnCode, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], 1);

        // Act
        await Mediator.Send(new CreateBook.Command(actor, command));

        // Assert
        var expected = GetExpectedDataAfterCreation(actor, command, isbnCode, CreatedAt, 1);
        DbContext.AssertData(1, expected);
        VerifyBookAuthor(1, [1]);
    }

    // 正常系_書名が100文字の境界値で登録できる
    [Fact]
    public async Task 正常系_書名が100文字の境界値で登録できる()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");

        var actor = UserFixture.Admin;
        string longestTitle = StringFixture.GetAsciiDummyString(100);
        var command = new BookCommandDTO("978-4-83-227072-5", longestTitle, new(2019, 2, 27), [1], 1);

        // Act
        await Mediator.Send(new CreateBook.Command(actor, command));

        // Assert
        var expected = GetExpectedDataAfterCreation(actor, command, "978-4-83-227072-5", CreatedAt, 1);
        DbContext.AssertData(1, expected);
        VerifyBookAuthor(1, [1]);
    }

    [Theory]
    [InlineData("9784832270725"), InlineData("4832270729")]
    public async Task 正常系_ISBNコードが数字のみ(string isbnDigitsOnly)
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");

        var actor = UserFixture.Admin;
        var command = new BookCommandDTO(isbnDigitsOnly, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], 1);

        // Act
        await Mediator.Send(new CreateBook.Command(actor, command));

        // Assert
        string expectedISBN = isbnDigitsOnly.Length == 13 ? "978-4-83-227072-5" : "4-83-227072-9";
        var expected = GetExpectedDataAfterCreation(actor, command, expectedISBN, CreatedAt, 1);
        DbContext.AssertData(1, expected);
        VerifyBookAuthor(1, [1]);
    }

    [Fact]
    public async Task 正常系_著者の順番が入力した順で保持される()
    {

        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");

        var actor = UserFixture.Admin;
        var command = new BookCommandDTO(
            "978-4-83-227072-5", "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [2, 1], 1
        );

        // Act
        await Mediator.Send(new CreateBook.Command(actor, command));

        // Assert
        var expected = GetExpectedDataAfterCreation(actor, command, "978-4-83-227072-5", CreatedAt, 1);
        DbContext.AssertData(1, expected);
        VerifyBookAuthor(1, [2, 1]);
    }

    [Fact]
    public async Task 異常系_一般ユーザーは登録できない()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");

        var actor = UserFixture.User1;
        var command = new BookCommandDTO(
            "978-4-83-227072-5", "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], 1
        );

        // Act
        var act = () => Mediator.Send(new CreateBook.Command(actor, command));

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        DbContext.AssertNotExistData<BookDataModel>();
    }

    // 異常系_空の書名だと登録できない
    [Theory]
    [InlineData(""), InlineData(null)]
    public async Task 異常系_空の書名だと登録できない(string emptyTitle)
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");

        var actor = UserFixture.Admin;
        var command = new BookCommandDTO("978-4-83-227072-5", emptyTitle, new(2019, 2, 27), [1], 1);

        // Act
        var act = () => Mediator.Send(new CreateBook.Command(actor, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("書名は空にできません。");
        DbContext.AssertNotExistData<BookDataModel>();
    }

    [Fact]
    public async Task 異常系_書名が101文字以上だと登録できない()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");

        var actor = UserFixture.Admin;
        string tooLongTitle = StringFixture.GetAsciiDummyString(101);
        var command = new BookCommandDTO("978-4-83-227072-5", tooLongTitle, new(2019, 2, 27), [1], 1);

        // Act
        var act = () => Mediator.Send(new CreateBook.Command(actor, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("書名は100文字以内で入力してください。");
        DbContext.AssertNotExistData<BookDataModel>();
    }

    [Fact]
    public async Task 異常系_存在しない出版社IDが指定されている()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");

        var actor = UserFixture.Admin;
        int notExistentPubId = 2;
        var command = new BookCommandDTO(
            "978-4-83-227072-5", "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], notExistentPubId
        );

        // Act
        var act = () => Mediator.Send(new CreateBook.Command(actor, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("存在しない出版社IDが指定されています。");
        DbContext.AssertNotExistData<BookDataModel>();
    }

    [Fact]
    public async Task 異常系_存在しない著者IDが含まれている()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");

        var actor = UserFixture.Admin;
        int[] invalidAuthorIds = [1, 2];
        var command = new BookCommandDTO(
            "978-4-83-227072-5", "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), invalidAuthorIds, 1
        );

        // Act
        var act = () => Mediator.Send(new CreateBook.Command(actor, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("存在しない著者IDが含まれています。");
        DbContext.AssertNotExistData<BookDataModel>();
    }

    [Fact]
    public async Task 異常系_著者IDが空()
    {
        // Arrange
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");

        var actor = UserFixture.Admin;
        int[] emptyAuthorIds = [];
        var command = new BookCommandDTO(
            "978-4-83-227072-5", "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), emptyAuthorIds, 1
        );

        // Act
        var act = () => Mediator.Send(new CreateBook.Command(actor, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("著者を1名以上指定してください。");
        DbContext.AssertNotExistData<BookDataModel>();
    }

    [Theory]
    [InlineData("978-4-83-227072-5"), InlineData("4-83-227072-9")]
    [InlineData("9784832270725"), InlineData("4832270729")]
    public async Task 異常系_既に登録されているISBNコードでは登録できない(string isbnCode)
    {
        // Arrange
        string formattedISBN = isbnCode switch
        {
            "9784832270725" => "978-4-83-227072-5",
            "4832270729" => "4-83-227072-9",
            { } isbn => isbn
        };

        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");
        DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, formattedISBN, "ぼっち・ざ・ろっく！(1)", [1], 1
        );

        var actor = UserFixture.Admin;
        var command = new BookCommandDTO(isbnCode, "青春コンプレックス", new(2019, 2, 27), [1], 1);

        // Act
        var act = () => Mediator.Send(new CreateBook.Command(actor, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("既に同じISBNコードの書籍が存在します。");
        DbContext.AssertNotExistData<BookDataModel>(2);
    }

    // 異常系_ISBNコードの形式が不正
    // 空, 桁数が不正 (ハイフンなし、あり), 数字以外が含まれている
    [Theory]
    [InlineData(""), InlineData(null)]
    [InlineData("1234567A91234")]
    [InlineData("123-4-56-789012-34"), InlineData("12345678901234")]
    [InlineData("1-23-456789-0-1"), InlineData("12345678901")]
    public async Task 異常系_ISBNコードの形式が不正(string invalidIsbn)
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");

        var actor = UserFixture.Admin;
        var command = new BookCommandDTO(
            invalidIsbn, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], 1
        );

        // Act
        var act = () => Mediator.Send(new CreateBook.Command(actor, command));

        // Assert
        string expectedErrorMessage = invalidIsbn switch
        {
            "" or null => "ISBNコードを入力してください。",
            "1234567A91234" => "ISBNコードに不正な文字が含まれています。",
            _ => "ISBNコードの桁数が違います。"
        };

        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage(expectedErrorMessage);
        DbContext.AssertNotExistData<BookDataModel>();
    }
}
