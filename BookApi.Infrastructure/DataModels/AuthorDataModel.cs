using BookApi.Domain.Entities;
using BookApi.Infrastructure.Abstractions.DataModels;
using BookApi.Infrastructure.DataModels.Intermediates;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.DataModels;

public class AuthorDataModel : AggregateDataModelBase<Author, AuthorDataModel>
{
    public string Name { get; set; } = string.Empty;
    public ICollection<BookDataModel> Books { get; set; } = null!;

    public override Author ToEntity()
        => new(
            ID,
            CreatedAt, UpdatedAt,
            CreatedByID, CreatedByName,
            UpdatedByID, UpdatedByName,
            VersionID,
            Name,
            [.. Books.Select(x => x.ID)]
        );

    protected override void OnTransferFromEntity(Author entity)
    {
        Name = entity.Name.Value;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthorDataModel>().HasIndex(x => x.Name).IsUnique();

        modelBuilder.Entity<BookAuthorDataModel>().ToTable("BookAuthor");
        modelBuilder.Entity<BookAuthorDataModel>().HasKey(x => new { x.AuthorID, x.BookID });

        // TODO: 中間テーブルは削除されるものの、著者を削除しても書籍は消えない
        modelBuilder.Entity<AuthorDataModel>()
            .HasMany(a => a.Books)
            .WithMany(b => b.Authors)
            .UsingEntity<BookAuthorDataModel>(
                l => l
                    .HasOne<BookDataModel>()
                    .WithMany()
                    .HasForeignKey(j => j.BookID)
                    .OnDelete(DeleteBehavior.Cascade),
                r => r
                    .HasOne<AuthorDataModel>()
                    .WithMany()
                    .HasForeignKey(j => j.AuthorID)
                    .OnDelete(DeleteBehavior.Cascade)
            );
    }
}
