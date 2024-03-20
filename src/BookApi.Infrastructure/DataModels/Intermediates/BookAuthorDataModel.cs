using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.DataModels.Intermediates;

public class BookAuthorDataModel
{
    public int BookId { get; set; }
    public int AuthorId { get; set; }
    public int Order { get; set; }

    public virtual BookDataModel Book { get; set; } = null!;
    public virtual AuthorDataModel Author { get; set; } = null!;

    internal static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookAuthorDataModel>().ToTable("BookAuthor");
        modelBuilder.Entity<BookAuthorDataModel>().HasKey(x => new { x.BookId, x.AuthorId });

        // NOTE: 書籍と著者が多対多の関係だと、著者を削除すると中間テーブルは削除されるものの、書籍そのものは削除されない！
        // 著者の削除時に書籍を削除する仕様はDBの制約で宣言できないため、手動で削除する必要がある。
        // 詳しくはBookDbContext内のSaveChangesAsync()のオーバーライドを参照。

        // ⚠ 下記のOnDeleteはあくまで中間テーブルに対してのみ有効
        modelBuilder.Entity<AuthorDataModel>()
            .HasMany(a => a.Books)
            .WithMany(b => b.Authors)
            .UsingEntity<BookAuthorDataModel>(
                l => l
                    .HasOne(j => j.Book)
                    .WithMany(b => b.BookAuthors)
                    .HasForeignKey(j => j.BookId)
                    .OnDelete(DeleteBehavior.Cascade),
                r => r
                    .HasOne(j => j.Author)
                    .WithMany(a => a.BookAuthors)
                    .HasForeignKey(j => j.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade)
            );
    }
}
