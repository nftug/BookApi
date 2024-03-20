using BookApi.Domain.Interfaces;
using BookApi.Infrastructure.Models;
using BookApi.Infrastructure.Services.QueryServices;
using BookApi.Infrastructure.Services.Repositories;
using BookApi.Infrastructure.Services.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration
    )
        => services
            .Configure<InitialUserSettings>(
                opt => configuration.GetSection(nameof(InitialUserSettings)).Bind(opt)
            )
            .AddSingleton<IPasswordService, PasswordService>()
            .AddScoped<IBookRepository, BookRepository>()
            .AddScoped<IAuthorRepository, AuthorRepository>()
            .AddScoped<IPublisherRepository, PublisherRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IBookQueryService, BookQueryService>()
            .AddScoped<IAuthorQueryService, AuthorQueryService>()
            .AddScoped<IPublisherQueryService, PublisherQueryService>()
            .AddScoped<IUserQueryService, UserQueryService>();
}
