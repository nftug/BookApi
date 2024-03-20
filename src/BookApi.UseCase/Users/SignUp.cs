using BookApi.Domain.DTOs.Commands;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Interfaces;
using BookApi.Domain.Services;
using MediatR;

namespace BookApi.UseCase.Users;

public class SignUp
{
    public record Command(SignUpCommandDTO CommandForm) : IRequest<LoginResponseDTO>;

    public class Handler(UserSaveService userSaveService, IAuthenticationService authService)
        : IRequestHandler<Command, LoginResponseDTO>
    {
        public async Task<LoginResponseDTO> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userSaveService.RegisterAsync(request.CommandForm);
            return authService.CreateToken(user);
        }
    }
}
