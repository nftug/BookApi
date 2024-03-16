using BookApi.Domain;
using BookApi.Infrastructure;
using BookApi.UseCase.Books;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services
    .AddDbContext<BookDbContext>(
        opt => opt
            .UseSqlite(configuration.GetConnectionString("DefaultConnection"))
            .UseLazyLoadingProxies()
    )
    .AddDomainServices()
    .AddInfrastructureServices()
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetBook).Assembly));

var app = builder.Build();

// Apply migration to DB
app.ApplyMigration();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
