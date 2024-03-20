using System.Text;
using BookApi.Domain.Interfaces;
using BookApi.Presentation.Models;
using BookApi.Presentation.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BookApi.Presentation;

public static class PresentationServiceExtensions
{
    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services, IConfiguration configuration
    )
    {
        var jwtSettings = new JwtSettings();
        var section = configuration.GetSection(nameof(JwtSettings));
        section.Bind(jwtSettings);

        services
            .Configure<JwtSettings>(section.Bind)
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<ActorFactoryService>();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                };
            });

        return services;
    }
}
