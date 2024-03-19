using BookApi.Domain.Entities;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;

namespace BookApi.Domain.Services;

public class UserSaveService(
    IUserRepository userRepository,
    IPasswordService passwordService,
    IDateTimeProvider dateTimeProvider
)
{
    public async Task<User> RegisterAsync(
        OwnerOnlyPermission permission,
        string rawUserId, string userName, string rawPassword
    )
    {
        var userId = UserId.CreateWithValidation(rawUserId);
        if (await userRepository.AnyByUserIdAsync(userId))
            throw new ValidationErrorException("既に同じユーザーIDが登録されています。");

        var newUser = User.CreateNew(
            permission, dateTimeProvider, passwordService,
            rawUserId, userName, rawPassword
        );
        await userRepository.SaveAsync(permission.Actor, newUser);
        return newUser;
    }

    public async Task ChangeUserNameAsync(OwnerOnlyPermission permission, User user, string userName)
    {
        user.ChangeUserName(permission, dateTimeProvider, userName);
        await userRepository.SaveAsync(permission.Actor, user);
    }

    public async Task ChangePasswordAsync(OwnerOnlyPermission permission, User user, string rawPassword)
    {
        user.ChangePassword(permission, dateTimeProvider, passwordService, rawPassword);
        await userRepository.SaveAsync(permission.Actor, user);
    }
}
