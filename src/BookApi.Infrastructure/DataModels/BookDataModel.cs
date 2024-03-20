using System.ComponentModel.DataAnnotations;
using BookApi.Domain.Entities;
using BookApi.Domain.ValueObjects.Books;
using BookApi.Infrastructure.Abstractions.DataModels;
using BookApi.Infrastructure.Attributes;
using BookApi.Infrastructure.DataModels.Intermediates;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.DataModels;

public class BookDataModel : AggregateDataModelBase<Book, BookDataModel>
{
    public string ISBN { get; set; } = string.Empty;
    [MaxLength(100)] public string Title { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public int PublisherId { get; set; }

    public virtual PublisherDataModel Publisher { get; set; } = null!;
    public virtual ICollection<AuthorDataModel> Authors { get; set; } = [];
    [Intermediate] public virtual ICollection<BookAuthorDataModel> BookAuthors { get; set; } = [];
    [Intermediate] public virtual ICollection<BookLikeDataModel> BookLikes { get; set; } = [];

    public override Book ToEntity()
        => new(
            Id,
            CreatedAt, UpdatedAt,
            CreatedByUserId, UpdatedByUserId,
            VersionId,
            Title,
            ISBN,
            [.. BookAuthors.OrderBy(x => x.Order).Select(x => x.AuthorId)],
            PublisherId,
            PublishedAt,
            [.. BookLikes.Select(x => BookLike.Reconstruct(x.UserId, x.LikedAt))]
        );

    protected override void OnTransferFromEntity(Book entity)
    {
        ISBN = entity.ISBN.Value;
        Title = entity.Title.Value;
        PublishedAt = entity.PublishedAt.Value;
        PublisherId = entity.Publisher.Value;
    }

    public override bool ReconstructIntermediates(Book entity)
    {
        BookAuthors = entity.Authors
            .Select((x, i) => new BookAuthorDataModel
            {
                BookId = Id,
                AuthorId = x.Value,
                Order = i
            })
            .ToList();

        BookLikes = entity.Likes
            .Select(x => new BookLikeDataModel
            {
                BookId = Id,
                UserId = x.LikedByItemId.Value,
                LikedAt = x.LikedAt
            })
            .ToList();

        return true;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookDataModel>().HasIndex(x => x.ISBN).IsUnique();
        BookAuthorDataModel.OnModelCreating(modelBuilder);
        BookLikeDataModel.OnModelCreating(modelBuilder);
    }
}
