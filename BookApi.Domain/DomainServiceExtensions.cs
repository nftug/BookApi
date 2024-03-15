using BookApi.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Domain;

public static class DomainServiceExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
        => services
            .AddTransient<BookSaveService>()
            .AddTransient<AuthorSaveService>()
            .AddTransient<PublisherSaveService>();
}
