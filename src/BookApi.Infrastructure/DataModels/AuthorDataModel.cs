using System.ComponentModel.DataAnnotations;
using BookApi.Domain.Entities;
using BookApi.Infrastructure.Abstractions.DataModels;
using BookApi.Infrastructure.Attributes;
using BookApi.Infrastructure.DataModels.Intermediates;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.DataModels;

public class AuthorDataModel : AggregateDataModelBase<Author, AuthorDataModel>
{
    [MaxLength(30)] public string Name { get; set; } = string.Empty;

    [CascadeDelete] public virtual ICollection<BookDataModel> Books { get; set; } = [];
    public virtual ICollection<BookAuthorDataModel> BookAuthors { get; set; } = [];

    public override Author ToEntity()
        => new(
            Id,
            CreatedAt, UpdatedAt,
            CreatedByUserId, UpdatedByUserId,
            VersionId,
            Name,
            [.. Books.Select(x => x.Id)]
        );

    protected override void OnTransferFromEntity(Author entity)
    {
        Name = entity.Name.Value;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthorDataModel>().HasIndex(x => x.Name).IsUnique();
    }
}
