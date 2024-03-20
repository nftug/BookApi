using System.Reflection;
using BookApi.Infrastructure.Attributes;
using BookApi.Infrastructure.DataModels;
using BookApi.Infrastructure.DataModels.Intermediates;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure;

public class BookDbContext : DbContext
{
    public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<BookDataModel> Books { get; set; } = null!;
    public DbSet<BookLikeDataModel> BookLikes { get; set; } = null!;
    public DbSet<AuthorDataModel> Authors { get; set; } = null!;
    public DbSet<PublisherDataModel> Publishers { get; set; } = null!;
    public DbSet<UserDataModel> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AuthorDataModel.CreateModel(modelBuilder);
        PublisherDataModel.CreateModel(modelBuilder);
        BookDataModel.CreateModel(modelBuilder);
        UserDataModel.CreateModel(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 保存前に共通で行う処理をここに記述
        foreach (var entry in ChangeTracker.Entries().ToList())
        {
            // 削除処理の際に関連するコレクションを削除する
            if (entry.State == EntityState.Deleted)
            {
                var relatedEntities = entry.Entity.GetType().GetProperties()
                    .Where(p => p.GetCustomAttribute<CascadeDeleteAttribute>() != null)
                    .Where(p => p.PropertyType.IsGenericType
                            && p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                    .Select(p => p.GetValue(entry.Entity))
                    .OfType<IEnumerable<object>>()
                    .SelectMany(e => e);

                foreach (var relatedEntity in relatedEntities.ToList())
                {
                    Entry(relatedEntity).State = EntityState.Deleted;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
