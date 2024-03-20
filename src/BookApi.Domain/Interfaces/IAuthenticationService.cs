using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Entities;

namespace BookApi.Domain.Interfaces;

public interface IAuthenticationService
{
    LoginResponseDTO CreateToken(User user);
}
