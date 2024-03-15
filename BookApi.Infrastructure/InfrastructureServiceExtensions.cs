using BookApi.Domain.Interfaces;
using BookApi.Infrastructure.Services.QueryServices;
using BookApi.Infrastructure.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration
    )
        => services
            .AddDbContext<BookDbContext>(
                opt => opt
                    .UseSqlite(configuration.GetConnectionString("DefaultConnection"))
                    .UseLazyLoadingProxies()
            )
            .AddScoped<IBookRepository, BookRepository>()
            .AddScoped<IAuthorRepository, AuthorRepository>()
            .AddScoped<IPublisherRepository, PublisherRepository>()
            .AddScoped<IBookQueryService, BookQueryService>()
            .AddScoped<IAuthorQueryService, AuthorQueryService>()
            .AddScoped<IPublisherQueryService, PublisherQueryService>();
}
