using BookApi.Domain.Entities;
using BookApi.Infrastructure.Abstractions.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.DataModels;

public class PublisherDataModel : AggregateDataModelBase<Publisher, PublisherDataModel>
{
    public string Name { get; set; } = string.Empty;
    public ICollection<BookDataModel> Books { get; set; } = null!;

    public override Publisher ToEntity()
        => new(
            ID,
            CreatedAt, UpdatedAt,
            CreatedByID, CreatedByName,
            UpdatedByID, UpdatedByName,
            VersionID,
            Name,
            [.. Books.Select(x => x.ID)]
        );

    protected override void OnTransferFromEntity(Publisher entity)
    {
        Name = entity.Name.Value;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PublisherDataModel>().HasIndex(x => x.Name).IsUnique();

        modelBuilder.Entity<PublisherDataModel>()
            .HasMany(a => a.Books)
            .WithOne(b => b.Publisher)
            .HasForeignKey(b => b.PublisherID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
