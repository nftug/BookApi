using BookApi.Domain.Entities;
using BookApi.Infrastructure.Abstractions.DataModels;
using BookApi.Infrastructure.Attributes;
using BookApi.Infrastructure.DataModels.Intermediates;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.DataModels;

public class BookDataModel : AggregateDataModelBase<Book, BookDataModel>
{
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public int PublisherID { get; set; }

    public virtual PublisherDataModel Publisher { get; set; } = null!;
    public virtual ICollection<AuthorDataModel> Authors { get; set; } = [];

    [IntermediateTable] public virtual ICollection<BookAuthorDataModel> BookAuthors { get; set; } = [];

    public override Book ToEntity()
        => new(
            ID,
            CreatedAt, UpdatedAt,
            CreatedByID, CreatedByName,
            UpdatedByID, UpdatedByName,
            VersionID,
            Title,
            ISBN,
            [.. Authors.Select(x => x.ID)],
            PublisherID,
            PublishedAt
        );

    protected override void OnTransferFromEntity(Book entity)
    {
        ISBN = entity.ISBN.Value;
        Title = entity.Title.Value;
        PublishedAt = entity.PublishedAt.Value;
        PublisherID = entity.Publisher.Value;
    }

    public override bool OnTransferAfterSave(Book entity)
    {
        BookAuthors = entity.Authors
            .Select(x => new BookAuthorDataModel { BookID = ID, AuthorID = x.Value })
            .ToList();
        return true;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookDataModel>().HasIndex(x => x.ISBN).IsUnique();
    }
}
