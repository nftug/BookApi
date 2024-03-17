using BookApi.Domain.DTOs.Commands;
using BookApi.UseCase.Books;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Test.Books;

public class UpdateBookTest : BookUseCaseTestBase
{
    public UpdateBookTest()
    {
        DateTimeProvider.SetupGet(x => x.UtcNow).Returns(UpdatedAt);

        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");
    }

    [Theory]
    [InlineData("978-4-83-227072-5"), InlineData("4-83-227072-9")]
    public async Task 正常系_書籍を更新(string isbnCode)
    {
        // Arrange
        var origin = DbContext.AddBookToDatabase(UserFixture.Admin, CreatedAt, DefaultISBN, "更新前", [2], 2);
        var actor = UserFixture.Admin;
        var command = new BookCommandDTO(isbnCode, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], 1);

        // Act
        await Mediator.Send(new UpdateBook.Command(actor, DefaultISBN, command));

        // Assert
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, isbnCode, UpdatedAt);
        AssertBookData(expected, command.AuthorIds);
    }

    [Fact]
    public async Task 正常系_書名が100文字の境界値で更新できる()
    {
        // Arrange
        string isbnCode = "978-4-83-227072-5";
        var origin = DbContext.AddBookToDatabase(UserFixture.Admin, CreatedAt, isbnCode, "更新前", [2], 2);
        var actor = UserFixture.Admin;
        string longestTitle = StringFixture.GetAsciiDummyString(100);
        var command = new BookCommandDTO(isbnCode, longestTitle, new(2019, 2, 27), [1], 1);

        // Act
        await Mediator.Send(new UpdateBook.Command(actor, isbnCode, command));

        // Assert
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, isbnCode, UpdatedAt);
        AssertBookData(expected, command.AuthorIds);
    }

    [Theory]
    [InlineData("9784832270725"), InlineData("4832270729")]
    public async Task 正常系_ハイフン無しのISBNコードをフォーマットして格納(string isbnDigitsOnly)
    {
        // Arrange
        var origin = DbContext.AddBookToDatabase(UserFixture.Admin, CreatedAt, DefaultISBN, "更新前", [2], 2);
        var actor = UserFixture.Admin;
        var command = new BookCommandDTO(isbnDigitsOnly, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], 1);

        // Act
        await Mediator.Send(new UpdateBook.Command(actor, DefaultISBN, command));

        // Assert
        string expectedISBN = isbnDigitsOnly.Length == 13 ? "978-4-83-227072-5" : "4-83-227072-9";
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, expectedISBN, UpdatedAt);
        AssertBookData(expected, command.AuthorIds);
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
        var origin = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, formattedISBN, "更新前", [2], 2
        );
        var actor = UserFixture.Admin;
        var command = new BookCommandDTO(formattedISBN, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], 1);

        // Act
        await Mediator.Send(new UpdateBook.Command(actor, isbnDigitsOnly, command));

        // Assert
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, formattedISBN, UpdatedAt);
        AssertBookData(expected, command.AuthorIds);
    }

    [Fact]
    public async Task 正常系_著者の順番が入力した順で保持される()
    {
        // Arrange
        string isbnCode = "978-4-83-227072-5";
        var origin = DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, DefaultISBN, "更新前", [2], 2
        );
        var actor = UserFixture.Admin;
        int[] authorIds = [2, 1];
        var command = new BookCommandDTO(isbnCode, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), authorIds, 1);

        // Act
        await Mediator.Send(new UpdateBook.Command(actor, DefaultISBN, command));

        // Assert
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, isbnCode, UpdatedAt);
        AssertBookData(expected, command.AuthorIds);
    }

    [Fact]
    public async Task 正常系_他の管理者が作成したデータを更新できる()
    {
        // Arrange
        string isbnCode = "978-4-83-227072-5";
        var origin = DbContext.AddBookToDatabase(UserFixture.Admin2, CreatedAt, DefaultISBN, "更新前", [2], 2);
        var actor = UserFixture.Admin;
        var command = new BookCommandDTO(isbnCode, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], 1);

        // Act
        await Mediator.Send(new UpdateBook.Command(actor, DefaultISBN, command));

        // Assert
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, isbnCode, UpdatedAt);
        AssertBookData(expected, command.AuthorIds);
    }

    [Fact]
    public async Task 正常系_取得時と同じISBNコードで更新できる()
    {
        // Arrange
        string isbnCode = "978-4-83-227072-5";
        var origin = DbContext.AddBookToDatabase(
            UserFixture.Admin2, CreatedAt, isbnCode, "更新前", [2], 2
        );
        var actor = UserFixture.Admin;
        var command = new BookCommandDTO(isbnCode, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], 1);

        // Act
        await Mediator.Send(new UpdateBook.Command(actor, isbnCode, command));

        // Assert
        var expected = GetExpectedDataAfterUpdate(origin, actor, command, isbnCode, UpdatedAt);
        AssertBookData(expected, command.AuthorIds);
    }

    [Fact]
    public async Task 異常系_一般ユーザーは登録できない()
    {
        // Arrange
        var origin = DbContext.AddBookToDatabase(UserFixture.Admin, CreatedAt, DefaultISBN, "更新前", [2], 2);
        var actor = UserFixture.User1;
        var command = new BookCommandDTO(DefaultISBN, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], 1);

        // Act
        var act = () => Mediator.Send(new UpdateBook.Command(actor, DefaultISBN, command));

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        AssertBookData(origin, [2]);
    }

    [Theory]
    [InlineData(""), InlineData(null)]
    public async Task 異常系_空の書名だと編集できない(string emptyTitle)
    {
        // Arrange
        var origin = DbContext.AddBookToDatabase(UserFixture.Admin, CreatedAt, DefaultISBN, "更新前", [2], 2);
        var actor = UserFixture.Admin;
        var command = new BookCommandDTO(DefaultISBN, emptyTitle, new(2019, 2, 27), [1], 1);

        // Act
        var act = () => Mediator.Send(new UpdateBook.Command(actor, DefaultISBN, command));

        // Assert
        await act.Should()
             .ThrowAsync<ValidationErrorException>()
             .WithMessage("書名は空にできません。");
        AssertBookData(origin, [2]);
    }

    [Fact]
    public async Task 異常系_書名が101文字以上だと登録できない()
    {
        // Arrange
        var origin = DbContext.AddBookToDatabase(UserFixture.Admin, CreatedAt, DefaultISBN, "更新前", [2], 2);
        var actor = UserFixture.Admin;
        string tooLongTitle = StringFixture.GetAsciiDummyString(101);
        var command = new BookCommandDTO(DefaultISBN, tooLongTitle, new(2019, 2, 27), [1], 1);

        // Act
        var act = () => Mediator.Send(new UpdateBook.Command(actor, DefaultISBN, command));

        // Assert
        await act.Should()
             .ThrowAsync<ValidationErrorException>()
             .WithMessage("書名は100文字以内で入力してください。");
        AssertBookData(origin, [2]);
    }

    [Fact]
    public async Task 異常系_存在しない出版社IDが指定されている()
    {
        // Arrange
        var origin = DbContext.AddBookToDatabase(UserFixture.Admin, CreatedAt, DefaultISBN, "更新前", [2], 2);
        var actor = UserFixture.Admin;
        int notExistentPubId = 100;
        var command =
            new BookCommandDTO(DefaultISBN, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], notExistentPubId);

        // Act
        var act = () => Mediator.Send(new UpdateBook.Command(actor, DefaultISBN, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("存在しない出版社IDが指定されています。");
        AssertBookData(origin, [2]);
    }

    [Fact]
    public async Task 異常系_存在しない著者IDが指定されている()
    {
        // Arrange
        var origin = DbContext.AddBookToDatabase(UserFixture.Admin, CreatedAt, DefaultISBN, "更新前", [2], 2);
        var actor = UserFixture.Admin;
        int[] invalidAuthorIds = [1, 2, 100];
        var command =
            new BookCommandDTO(DefaultISBN, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), invalidAuthorIds, 2);

        // Act
        var act = () => Mediator.Send(new UpdateBook.Command(actor, DefaultISBN, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("存在しない著者IDが含まれています。");
        AssertBookData(origin, [2]);
    }

    [Fact]
    public async Task 異常系_著者IDが空()
    {
        // Arrange
        var origin = DbContext.AddBookToDatabase(UserFixture.Admin, CreatedAt, DefaultISBN, "更新前", [2], 2);
        var actor = UserFixture.Admin;
        int[] emptyAuthorIds = [];
        var command =
            new BookCommandDTO(DefaultISBN, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), emptyAuthorIds, 2);

        // Act
        var act = () => Mediator.Send(new UpdateBook.Command(actor, DefaultISBN, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("著者を1名以上指定してください。");
        AssertBookData(origin, [2]);
    }

    [Fact]
    public async Task 異常系_出版日が空()
    {
        // Arrange
        var origin = DbContext.AddBookToDatabase(UserFixture.Admin, CreatedAt, DefaultISBN, "更新前", [2], 2);
        var actor = UserFixture.Admin;
        DateTime emptyDateTime = default;
        var command =
            new BookCommandDTO(DefaultISBN, "ぼっち・ざ・ろっく！(1)", emptyDateTime, [1], 2);

        // Act
        var act = () => Mediator.Send(new UpdateBook.Command(actor, DefaultISBN, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("出版日が不正です。");
        AssertBookData(origin, [2]);
    }

    [Theory]
    [InlineData("978-4-83-227072-5"), InlineData("4-83-227072-9")]
    [InlineData("9784832270725"), InlineData("4832270729")]
    public async Task 異常系_既に登録されているISBNコードでは更新できない(string isbnCode)
    {
        // Arrange
        string formattedISBN = isbnCode switch
        {
            "9784832270725" => "978-4-83-227072-5",
            "4832270729" => "4-83-227072-9",
            { } isbn => isbn
        };
        DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt, formattedISBN, "ぼっち・ざ・ろっく！(1)", [1], 1
        );

        var origin = DbContext.AddBookToDatabase(UserFixture.Admin, CreatedAt, DefaultISBN, "更新前", [2], 2);
        var actor = UserFixture.Admin;
        var command =
            new BookCommandDTO(isbnCode, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], 2);

        // Act
        var act = () => Mediator.Send(new UpdateBook.Command(actor, DefaultISBN, command));

        // Assert
        await act.Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("既に同じISBNコードの書籍が存在します。");
        AssertBookData(origin, [2]);
    }

    [Theory]
    [InlineData(""), InlineData(null)]
    [InlineData("1234567A91234")]
    [InlineData("123-4-56-789012-34"), InlineData("12345678901234")]
    [InlineData("1-23-456789-0-1"), InlineData("12345678901")]
    public async Task 異常系_ISBNコードの形式が不正(string invalidIsbn)
    {
        // Arrange
        var origin = DbContext.AddBookToDatabase(UserFixture.Admin, CreatedAt, DefaultISBN, "更新前", [2], 2);
        var actor = UserFixture.Admin;
        var command = new BookCommandDTO(invalidIsbn, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], 1);

        // Act
        var act = () => Mediator.Send(new UpdateBook.Command(actor, DefaultISBN, command));

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
        AssertBookData(origin, [2]);
    }

    [Fact]
    public async Task 異常系_対象の書籍が見つからない()
    {
        // Arrange
        var actor = UserFixture.Admin;
        string notExistentISBN = "123-4-56-789012-3";
        var command = new BookCommandDTO(notExistentISBN, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], 1);

        // Act
        var act = () => Mediator.Send(new UpdateBook.Command(actor, notExistentISBN, command));

        // Assert
        await act.Should().ThrowAsync<ItemNotFoundException>();
    }

    [Fact]
    public async Task 異常系_楽観的排他制御による編集ロック()
    {
        // Arrange mock services
        var serviceProviderWithLocalMock = BuildServiceProviderForConcurrencyTest();

        // Arrange data
        DbContext.AddBookToDatabase(UserFixture.Admin, CreatedAt, DefaultISBN, "更新前", [2], 2);
        var actor = UserFixture.Admin;
        var command = new BookCommandDTO(DefaultISBN, "ぼっち・ざ・ろっく！(1)", new(2019, 2, 27), [1], 1);

        // Act
        var act = () =>
            serviceProviderWithLocalMock
                .GetRequiredService<ISender>()
                .Send(new UpdateBook.Command(actor, DefaultISBN, command));

        // Assert
        await act.Should().ThrowAsync<ConcurrencyException>();
        var actual = DbContext.Books.Single(x => x.ISBN == DefaultISBN);
        actual.VersionId.Should().Be(1);
    }
}
