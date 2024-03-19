using System.ComponentModel.DataAnnotations;
using BookApi.Domain.Entities;
using BookApi.Domain.Enums;
using BookApi.Infrastructure.Abstractions.DataModels;
using BookApi.Infrastructure.DataModels.Intermediates;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Infrastructure.DataModels;

public class UserDataModel : AggregateDataModelBase<User, UserDataModel>
{
    [MaxLength(20)] public string UserId { get; set; } = string.Empty;
    [MaxLength(30)] public string UserName { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    public virtual ICollection<BookLikeDataModel> BookLikes { get; set; } = [];

    public override User ToEntity()
        => new(
            Id,
            CreatedAt, UpdatedAt,
            CreatedByUserId, UpdatedByUserId,
            VersionId,
            UserId,
            UserName,
            HashedPassword,
            Role
        );

    protected override void OnTransferFromEntity(User entity)
    {
        UserId = entity.UserId.Value;
        UserName = entity.UserName.Value;
        HashedPassword = entity.HashedPassword.Value;
        Role = entity.Role;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserDataModel>().HasIndex(x => x.UserId).IsUnique();
    }
}
