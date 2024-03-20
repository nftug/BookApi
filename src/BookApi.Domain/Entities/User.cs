using BookApi.Domain.Abstractions.Entities;
using BookApi.Domain.Enums;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;

namespace BookApi.Domain.Entities;

public class User : AggregateEntityBase<User>
{
    public UserId UserId { get; }
    public UserName UserName { get; private set; }
    public HashedPassword HashedPassword { get; private set; }
    public UserRole Role { get; private set; }

    public bool IsAdmin => Role == UserRole.Admin;

    public User(
        int id,
        DateTime createdAt, DateTime? updatedAt,
        string createdByUserId, string? updatedByUserId,
        int versionId,
        string userId,
        string userName,
        string hashedPassword,
        UserRole role
    ) : base(id, createdAt, updatedAt, createdByUserId, updatedByUserId, versionId)
    {
        UserId = UserId.Reconstruct(userId);
        UserName = UserName.Reconstruct(userName);
        HashedPassword = HashedPassword.Reconstruct(hashedPassword);
        Role = role;
    }

    private User(IPasswordService passwordService, string userId, string userName, string rawPassword)
    {
        UserId = UserId.CreateWithValidation(userId);
        UserName = UserName.CreateWithValidation(userName);
        HashedPassword = HashedPassword.CreateWithValidation(passwordService, rawPassword);
    }

    internal static User CreateNew(
        PassThroughPermission permission,
        IDateTimeProvider dateTimeProvider, IPasswordService passwordService,
        string userId, string userName, string rawPassword
    )
        => new User(passwordService, userId, userName, rawPassword)
            .CreateNew(permission, dateTimeProvider);

    internal void ChangeUserName(
        OwnerOnlyPermission permission,
        IDateTimeProvider dateTimeProvider, string name
    )
    {
        UserName = UserName.CreateWithValidation(name);
        Update(permission, dateTimeProvider);
    }

    internal void ChangePassword(
        OwnerOnlyPermission permission,
        IDateTimeProvider dateTimeProvider, IPasswordService passwordService,
        string oldPasswordRaw, string newPasswordRaw
    )
    {
        HashedPassword = HashedPassword.ChangePassword(passwordService, oldPasswordRaw, newPasswordRaw);
        Update(permission, dateTimeProvider);
    }

    public bool VerifyPassword(IPasswordService passwordService, string rawPassword)
        => HashedPassword.VerifyPassword(passwordService, rawPassword);

    public Actor ToActor() => new(ItemId, UserId.Value, IsAdmin);
}
