using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace BookApi.Test.Authors;

public class AuthorInfrastructureTest : AuthorUseCaseTestBase
{
    [Fact]
    public void 異常系_名前欄がDBのユニーク制約に違反()
    {
        // Arrange
        AddDataToDatabase(UserFixture.Admin, "後藤ひとり", CreatedAt);

        // Act
        // ドメイン層のバリデーションを経由せずに、直接データを更新する
        var act = () => AddDataToDatabase(UserFixture.Admin, "後藤ひとり", UpdatedAt);

        // Assert
        act.Should()
            .Throw<DbUpdateException>()
            .WithInnerException<SqliteException>()
            .Which.SqliteErrorCode.Should().Be(19);

        var actual = DbContext.Authors.Single(x => x.Id == 1);
        actual.VersionId.Should().Be(0);
    }
}
