using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Test.Publishers;

public class PublisherInfrastructureTest : PublisherUseCaseTestBase
{
    [Fact]
    public void 異常系_名前欄がDBのユニーク制約に違反()
    {
        // Arrange
        DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");

        // Act
        // ドメイン層のバリデーションを経由せずに、直接データを更新する
        var act = () => DbContext.AddPublisherToDatabase(UserFixture.Admin, CreatedAt, "スターリー出版");

        // Assert
        act.Should()
            .Throw<DbUpdateException>()
            .WithInnerException<SqliteException>()
            .Which.SqliteErrorCode.Should().Be(19);

        var actual = DbContext.Publishers.Single(x => x.Id == 1);
        actual.VersionId.Should().Be(0);
    }
}
