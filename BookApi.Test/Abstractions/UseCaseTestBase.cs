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

    protected ISender Mediator => ServiceProvider.GetRequiredService<ISender>();
    protected BookDbContext DbContext => ServiceProvider.GetRequiredService<BookDbContext>();

    protected readonly Mock<IDateTimeProvider> DateTimeProvider = new();

    // setup
    protected UseCaseTestBase()
    {
        // テストケースごとにSQLiteのインメモリDBを作成する
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        // DIコンテナの設定
        ServiceProvider =
            new ServiceCollection()
                .AddDbContext<BookDbContext>(
                    opt => opt
                        .UseSqlite(_connection)
                        .UseLazyLoadingProxies()
                )
                .AddDomainServices()
                .AddInfrastructureServices()
                .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetBook).Assembly))
                .AddSingleton(_ => DateTimeProvider.Object)
                .BuildServiceProvider();

        // DBの初期化
        using var temporaryScope = ServiceProvider.CreateScope();
        var dbContext = temporaryScope.ServiceProvider.GetRequiredService<BookDbContext>();
        dbContext.Database.EnsureCreated();

        // ここに共通のシード処理を書く
    }

    // teardown
    public void Dispose()
    {
        // 接続が解除されるとインメモリDBも破棄される
        _connection.Dispose();
    }
}
