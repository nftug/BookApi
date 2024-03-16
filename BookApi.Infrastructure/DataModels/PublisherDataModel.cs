using BookApi.Domain.Entities;
using BookApi.Infrastructure.Abstractions.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.DataModels;

public class PublisherDataModel : AggregateDataModelBase<Publisher, PublisherDataModel>
{
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<BookDataModel> Books { get; set; } = [];

    public override Publisher ToEntity()
        => new(
            Id,
            CreatedAt, UpdatedAt,
            CreatedById, CreatedByName,
            UpdatedById, UpdatedByName,
            VersionId,
            Name,
            [.. Books.Select(x => x.Id)]
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
            .HasForeignKey(b => b.PublisherId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
