using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.DataModels.Intermediates;

public class BookLikeDataModel
{
    public int BookId { get; set; }
    public int UserId { get; set; }
    public DateTime LikedAt { get; set; }

    public virtual BookDataModel Book { get; set; } = null!;
    public virtual UserDataModel User { get; set; } = null!;

    internal static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookLikeDataModel>().ToTable("BookLike");
        modelBuilder.Entity<BookLikeDataModel>().HasKey(x => new { x.BookId, x.UserId });

        modelBuilder.Entity<BookDataModel>()
            .HasMany<UserDataModel>()
            .WithMany()
            .UsingEntity<BookLikeDataModel>(
                l => l
                    .HasOne(j => j.User)
                    .WithMany(u => u.BookLikes)
                    .HasForeignKey(j => j.UserId)
                    .OnDelete(DeleteBehavior.Cascade),
                r => r
                    .HasOne(j => j.Book)
                    .WithMany(b => b.BookLikes)
                    .HasForeignKey(j => j.BookId)
                    .OnDelete(DeleteBehavior.Cascade)
            );
    }
}
