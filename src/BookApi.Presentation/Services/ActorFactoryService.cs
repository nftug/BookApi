using System.Security.Claims;
using BookApi.Domain.Exceptions;
using BookApi.Domain.Interfaces;
using BookApi.Domain.ValueObjects.Shared;
using BookApi.Domain.ValueObjects.Users;

namespace BookApi.Presentation.Services;

public class ActorFactoryService(
    IHttpContextAccessor httpContextAccessor, IUserRepository userRepository
)
{
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<ActorForPermission> GetActorAsync()
    {
        string userId =
            _httpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var user =
            await userRepository.FindByUserIdAsync(UserId.Reconstruct(userId))
            ?? throw new UnauthorizedException();

        return user.ToActor();
    }
}
