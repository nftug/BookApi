using BookApi.Domain.DTOs.Commands;
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
    public async Task<User> RegisterAsync(SignUpCommandDTO command)
    {
        var userId = UserId.CreateWithValidation(command.UserId);
        if (await userRepository.AnyByUserIdAsync(userId))
            throw new ValidationErrorException("既に同じユーザーIDが登録されています。");

        // 一時的なPermissionを作成
        var actor = new ActorForPermission(ItemId.Reconstruct(0), userId.Value, false);
        var permission = new PassThroughPermission(actor);

        var newUser = User.CreateNew(
            permission, dateTimeProvider, passwordService,
            command.UserId, command.UserName, command.Password
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
