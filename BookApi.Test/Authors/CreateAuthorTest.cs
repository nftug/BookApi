using BookApi.Domain.DTOs.Commands;
using BookApi.Infrastructure.DataModels;
using BookApi.UseCase.Authors;

namespace BookApi.Test.Authors;

public class CreateAuthorTest : UseCaseTestBase
{
    [Fact]
    public async Task 正常系_著者を登録()
    {
        // Arrange
        var actor = UserFixture.Admin;
        var command = new AuthorCommandDTO("テスト太郎");
        var expected = new AuthorDataModel
        {
            Id = 1,
            Name = "テスト太郎"
        };

        // Act
        var result = await Mediator.Send(new CreateAuthor.Command(actor, command));

        // Assert
        var actual = DbContext.Authors.Single(x => x.Id == result.ItemId);
        actual.Should().BeEquivalentTo(expected);
    }
}
