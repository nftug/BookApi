using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;
using MediatR;

namespace BookApi.UseCase.Users;

public class DeleteUser
{
    public record Command(Actor Actor, string UserId) : IRequest;

    public class Handler(IUserRepository userRepository) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = UserId.Reconstruct(request.UserId);
            var user =
                await userRepository.FindByUserIdAsync(userId)
                ?? throw new ItemNotFoundException();

            if (user.Id == 1)
                throw new ValidationErrorException("初期ユーザーは削除できません。");

            var permission = new OwnerOnlyPermission(request.Actor, user);

            await userRepository.DeleteAsync(permission, user);
        }
    }
}
