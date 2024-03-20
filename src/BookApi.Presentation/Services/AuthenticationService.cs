using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookApi.Domain.DTOs.Responses;
using BookApi.Domain.Entities;
using BookApi.Domain.Interfaces;
using BookApi.Presentation.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BookApi.Presentation.Services;

public class AuthenticationService(IOptions<JwtSettings> options) : IAuthenticationService
{
    private readonly JwtSettings _jwtSettings = options.Value;

    public LoginResponseDTO CreateToken(User user)
    {
        // トークンを発行
        var claims = new List<Claim> {
            new(ClaimTypes.Name, user.UserName.Value),
            new(ClaimTypes.NameIdentifier, user.UserId.Value),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(_jwtSettings.ExpireDays),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new(user.UserId.Value, tokenHandler.WriteToken(token));
    }
}
