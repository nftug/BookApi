using BookApi.Domain.Interfaces;
using BookApi.Infrastructure.Services.QueryServices;
using BookApi.Infrastructure.Services.Repositories;
using BookApi.Infrastructure.Services.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        => services
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
