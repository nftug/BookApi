using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookApi.Infrastructure;

public static class InfrastructureHostExtensions
{
    public static void ApplyMigration(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookDbContext>();
        dbContext.Database.Migrate();
    }
}
