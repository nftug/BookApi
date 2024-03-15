using BookApi.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure;

public class BookDbContext : DbContext
{
    public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<BookDataModel> Books { get; set; } = null!;
    public DbSet<AuthorDataModel> Authors { get; set; } = null!;
    public DbSet<PublisherDataModel> Publishers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AuthorDataModel.CreateModel(modelBuilder);
        PublisherDataModel.CreateModel(modelBuilder);
        BookDataModel.CreateModel(modelBuilder);
    }
}
