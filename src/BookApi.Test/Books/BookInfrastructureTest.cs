using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Test.Books;

public class BookInfrastructureTest : BookUseCaseTestBase
{
    [Fact]
    public void 異常系_ISBNがDBのユニーク制約に違反()
    {
        // Arrange
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "はまじあき");
        DbContext.AddAuthorToDatabase(UserFixture.Admin, CreatedAt, "後藤ひとり");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "芳文社");
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");

        DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt,
            "978-4-83-227072-5",
            "ぼっち・ざ・ろっく！(1)",
            [1], 1,
            new(2019, 2, 27)
        );

        // Act
        // ドメイン層のバリデーションを経由せずに、直接データを追加する
        var act = () => DbContext.AddBookToDatabase(
            UserFixture.Admin, CreatedAt,
            "978-4-83-227072-5",
            "Can you add this book?",
            [2], 2,
            new(2020, 2, 27)
        );

        // Assert
        act.Should()
            .Throw<DbUpdateException>()
            .WithInnerException<SqliteException>()
            .Which.SqliteErrorCode.Should().Be(19);

        var actual = DbContext.Books.Single(x => x.Id == 1);
        actual.VersionId.Should().Be(0);
    }
}
