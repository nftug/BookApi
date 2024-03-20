using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Users;
using MediatR;

namespace BookApi.UseCase.Users;

public class Login
{
    public record Command(LoginCommandDTO CommandForm) : IRequest<LoginResponseDTO>;

    public class Handler(
        IPasswordService passwordService,
        IUserRepository userRepository,
        IAuthenticationService authService
    ) : IRequestHandler<Command, LoginResponseDTO>
    {
        public async Task<LoginResponseDTO> Handle(Command request, CancellationToken cancellationToken)
        {
            // ユーザーを認証
            var user =
                await userRepository.FindByUserIdAsync(UserId.Reconstruct(request.CommandForm.UserId))
                ?? throw new UnauthorizedException();

            if (!user.VerifyPassword(passwordService, request.CommandForm.Password))
                throw new UnauthorizedException();

            return authService.CreateToken(user);
        }
    }
}
