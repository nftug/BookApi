using BookApi.Domain;
using BookApi.Domain.Interfaces;
using BookApi.Infrastructure;
using BookApi.UseCase.Books;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookApi.Test.Abstractions;

public abstract class UseCaseTestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly DbContextOptions<BookDbContext> DbContextOptions;

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
        DbContextOptions = new DbContextOptionsBuilder<BookDbContext>()
            .UseSqlite(_connection)
            .UseLazyLoadingProxies()
            .Options;

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
                .AddInfrastructureServices()
                .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetBook).Assembly))
                .AddSingleton(_ => DateTimeProvider.Object);

    // teardown
    public void Dispose()
    {
        // 接続が解除されるとインメモリDBも破棄される
        _connection.Dispose();
    }
}