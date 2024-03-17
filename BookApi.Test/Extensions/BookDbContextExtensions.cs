using System.Linq.Expressions;
using BookApi.Domain.Abstractions.ValueObjects;
using BookApi.Infrastructure;
using BookApi.Infrastructure.Abstractions.DataModels;
using BookApi.Infrastructure.DataModels;
using BookApi.Infrastructure.DataModels.Intermediates;

namespace BookApi.Test.Extensions;

public static class BookDbContextForTestExtensions
{
    public static AuthorDataModel AddAuthorToDatabase(
        this BookDbContext dbContext, IActor createdBy, DateTime createdAt, string authorName
    )
    {
        var data = new AuthorDataModel
        {
            Name = authorName,
            CreatedAt = createdAt,
            CreatedById = createdBy.UserId,
            CreatedByName = createdBy.UserName
        };
        dbContext.Add(data);
        dbContext.SaveChanges();
        dbContext.ChangeTracker.Clear();
        return data;
    }

    public static PublisherDataModel AddPublisherToDatabase(
        this BookDbContext dbContext, IActor createdBy, DateTime createdAt, string publisherName
    )
    {
        var data = new PublisherDataModel
        {
            Name = publisherName,
            CreatedAt = createdAt,
            CreatedById = createdBy.UserId,
            CreatedByName = createdBy.UserName
        };
        dbContext.Add(data);
        dbContext.SaveChanges();
        dbContext.ChangeTracker.Clear();
        return data;
    }

    public static BookDataModel AddBookToDatabase(
        this BookDbContext dbContext, IActor createdBy, DateTime createdAt,
        string isbn, string title, int[] authorIds, int publisherId
    )
    {
        var data = new BookDataModel
        {
            ISBN = isbn,
            Title = title,
            PublisherId = publisherId,
            PublishedAt = default,
            CreatedAt = createdAt,
            CreatedById = createdBy.UserId,
            CreatedByName = createdBy.UserName
        };
        dbContext.Add(data);
        dbContext.SaveChanges();

        dbContext.AddRange(
            authorIds
                .Select((x, i) => new BookAuthorDataModel { BookId = data.Id, AuthorId = x, Order = i })
        );
        dbContext.SaveChanges();

        dbContext.ChangeTracker.Clear();

        return data;
    }

    public static void AssertData<TDataModel>(this BookDbContext dbContext, int itemId, TDataModel expected)
        where TDataModel : class, IDataModel
    {
        // 事前に関連するテーブルの遅延ロードを無効にする
        bool lazyLoadingEnabled = dbContext.ChangeTracker.LazyLoadingEnabled;
        dbContext.ChangeTracker.LazyLoadingEnabled = false;

        var actual = dbContext.Set<TDataModel>().Single(x => x.Id == itemId);
        actual.Should().BeEquivalentTo(expected);

        dbContext.ChangeTracker.LazyLoadingEnabled = lazyLoadingEnabled;
    }

    public static void AssertNotExistData<TDataModel>(this BookDbContext dbContext, int? itemId = null)
         where TDataModel : class, IDataModel
    {
        bool isExisting = dbContext.Set<TDataModel>().Any(x => itemId == null || x.Id == itemId);
        isExisting.Should().BeFalse();
    }

    public static void AssertNotExistData<TDataModel>(
        this BookDbContext dbContext, Expression<Func<TDataModel, bool>> predicate
    )
        where TDataModel : class
    {
        bool isExisting = dbContext.Set<TDataModel>().Any(predicate);
        isExisting.Should().BeFalse();
    }

    public static void AssertExistData<TDataModel>(
        this BookDbContext dbContext, Expression<Func<TDataModel, bool>> predicate
    )
        where TDataModel : class
    {
        bool isExisting = dbContext.Set<TDataModel>().Any(predicate);
        isExisting.Should().BeTrue();
    }
}
