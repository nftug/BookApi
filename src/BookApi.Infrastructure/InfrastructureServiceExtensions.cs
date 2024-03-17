using BookApi.Domain.Interfaces;
using BookApi.Infrastructure.Services.QueryServices;
using BookApi.Infrastructure.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        => services
            .AddScoped<IBookRepository, BookRepository>()
            .AddScoped<IAuthorRepository, AuthorRepository>()
            .AddScoped<IPublisherRepository, PublisherRepository>()
            .AddScoped<IBookQueryService, BookQueryService>()
            .AddScoped<IAuthorQueryService, AuthorQueryService>()
            .AddScoped<IPublisherQueryService, PublisherQueryService>();
}
