using BookApi.Domain.Enums;
using BookApi.Domain.Interfaces;
using BookApi.Infrastructure.DataModels;
using BookApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BookApi.Infrastructure;

public static class InfrastructureHostExtensions
{
    public static void ApplyMigration(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookDbContext>();
        dbContext.Database.Migrate();
    }

    public static void SeedInitialAdminUser(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<BookDbContext>();
        var passwordService = serviceProvider.GetRequiredService<IPasswordService>();
        var settings = serviceProvider.GetRequiredService<IOptions<InitialUserSettings>>().Value;

        // Seed admin user
        if (dbContext.Users.Any(x => x.UserId == settings.UserId.ToLower())) return;

        var adminUser =
            new UserDataModel
            {
                UserId = settings.UserId,
                UserName = settings.UserName,
                HashedPassword = passwordService.HashPassword(settings.Password),
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            };

        dbContext.Add(adminUser);
        dbContext.SaveChanges();
    }
}
