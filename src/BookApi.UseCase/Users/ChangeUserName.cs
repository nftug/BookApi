using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.Services;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;
using MediatR;

namespace BookApi.UseCase.Users;

public class ChangeUserName
{
    public record Command(Actor Actor, UserNameCommandDTO CommandForm) : IRequest;

    public class Handler(UserSaveService userSaveService, IUserRepository userRepository)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var user =
                await userRepository.FindByUserIdAsync(UserId.Reconstruct(request.Actor.UserId))
                ?? throw new ItemNotFoundException();
            var permission = new OwnerOnlyPermission(request.Actor, user);

            await userSaveService.ChangeUserNameAsync(permission, user, request.CommandForm);
        }
    }
}
