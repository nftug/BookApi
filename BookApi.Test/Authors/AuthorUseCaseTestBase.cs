using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Domain.DTOs.Commands;
using BookApi.Infrastructure.DataModels;

namespace BookApi.Test.Authors;

public abstract class AuthorUseCaseTestBase : UseCaseTestBase
{
    protected static readonly DateTime CreatedAt = new(2024, 3, 16, 20, 0, 0);
    protected static readonly DateTime UpdatedAt = new(2024, 3, 17, 20, 0, 0);

    protected void AssertNotCreatedAuthor(int newItemId = 1)
    {
        bool isExistedAuthor = DbContext.Authors.Any(x => x.Id == newItemId);
        isExistedAuthor.Should().BeFalse();
    }

    protected static AuthorDataModel GetExpectedDataAfterCreation(
        IActor createdBy, AuthorCommandDTO command, DateTime createdAt, int itemId = 1
    )
        => new()
        {
            Id = itemId,
            Name = command.Name,
            CreatedAt = createdAt,
            CreatedById = createdBy.UserId,
            CreatedByName = createdBy.UserName
        };

    protected static AuthorDataModel GetExpectedDataAfterUpdate(
        AuthorDataModel originData,
        IActor updatedBy,
        AuthorCommandDTO command,
        DateTime updatedAt
    )
        => new()
        {
            Id = originData.Id,
            Name = command.Name,
            CreatedAt = originData.CreatedAt,
            CreatedById = originData.CreatedById,
            CreatedByName = originData.CreatedByName,
            UpdatedAt = updatedAt,
            UpdatedById = updatedBy.UserId,
            UpdatedByName = updatedBy.UserName,
            VersionId = originData.VersionId + 1
        };

    protected AuthorDataModel ArrangeStoredData(
        IActor createdBy, string authorName, DateTime createdAt, int itemId = 1
    )
    {
        var data = new AuthorDataModel
        {
            Id = itemId,
            Name = authorName,
            CreatedAt = createdAt,
            CreatedById = createdBy.UserId,
            CreatedByName = createdBy.UserName
        };
        DbContext.Add(data);
        DbContext.SaveChanges();
        return data;
    }

    protected void AssertData(int itemId, AuthorDataModel expected)
    {
        var actual = DbContext.Authors.Single(x => x.Id == itemId);
        actual.Should().BeEquivalentTo(expected);
    }
}
