using BookApi.Domain.Entities;
using BookApi.Infrastructure.Abstractions.DataModels;
using BookApi.Infrastructure.Attributes;
using BookApi.Infrastructure.DataModels.Intermediates;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.DataModels;

public class AuthorDataModel : AggregateDataModelBase<Author, AuthorDataModel>
{
    public string Name { get; set; } = string.Empty;

    [CascadeDelete] public virtual ICollection<BookDataModel> Books { get; set; } = [];

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

        // NOTE: 書籍と著者が多対多の関係だと、著者を削除すると中間テーブルは削除されるものの、書籍そのものは削除されない！
        // 著者の削除時に書籍を削除する仕様はDBの制約で宣言できないため、手動で削除する必要がある。
        // 詳しくはBookDbContext内のSaveChangesAsync()のオーバーライドを参照。

        // ⚠ 下記のOnDeleteはあくまで中間テーブルに対してのみ有効
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
