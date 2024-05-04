using System.Text;
using BookApi.Domain;
using BookApi.Domain.Interfaces;
using BookApi.Infrastructure;
using BookApi.UseCase.Books;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Test.Abstractions;

public abstract class UseCaseTestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly DbContextOptions<BookDbContext> DbContextOptions;
    protected readonly IConfiguration Configuration;

    protected BookDbContext DbContext => ServiceProvider.GetRequiredService<BookDbContext>();
    protected ISender Mediator => ServiceProvider.GetRequiredService<ISender>();

    protected readonly Mock<IDateTimeProvider> DateTimeProvider = new();

    // setup
    protected UseCaseTestBase()
    {
        // テストケースごとにSQLiteのインメモリDBを作成する
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        // DbContextのオプションを構築
        var dbContextOptionBuilder =

        DbContextOptions = new DbContextOptionsBuilder<BookDbContext>()
            .UseSqlite(_connection, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            .UseLazyLoadingProxies()
            .Options;

        // Configurationを構築
        string appSettingsJson =
"""
{
  "HashSalts": {
    "PasswordSalt": "6d713bded597f3de0954a88d1b45598459d2446ccdd56c2be8330de2ede8b256"
  },
  "InitialUserSettings": {
    "UserId": "admin",
    "UserName": "Admin",
    "Password": "password"
  }
}
""";
        Configuration = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettingsJson)))
            .Build();

        // DIコンテナの設定
        ServiceProvider =
            BuildServiceCollectionBase()
                .AddScoped(_ => new BookDbContext(DbContextOptions))
                .BuildServiceProvider();

        // DBの初期化
        using var dbContext = new BookDbContext(DbContextOptions);
        dbContext.Database.EnsureCreated();

        // ここに共通のシード処理を書く
    }

    protected IServiceCollection BuildServiceCollectionBase()
        => new ServiceCollection()
                .AddDomainServices()
                .AddInfrastructureServices(Configuration, DbContextOptions)
                .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetBook).Assembly))
                .AddSingleton(_ => DateTimeProvider.Object);

    // teardown
    public void Dispose()
    {
        // 接続が解除されるとインメモリDBも破棄される
        _connection.Dispose();
    }
}
