using BookApi.Domain;
using BookApi.Infrastructure;
using BookApi.Presentation;
using BookApi.Presentation.Models;
using BookApi.UseCase.Books;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.OperationFilter<AllowAnonymousOperationFilter>();
});

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

var dbContextOptions = new DbContextOptionsBuilder<BookDbContext>()
    .UseNpgsql(
        configuration.GetConnectionString("DefaultConnection"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    )
    .UseLazyLoadingProxies()
    .Options;

builder.Services
    .AddDomainServices()
    .AddInfrastructureServices(configuration, dbContextOptions)
    .AddPresentationServices(configuration)
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetBook).Assembly));

var app = builder.Build();

// Apply migration to DB and seed data
app.ApplyMigration();
app.SeedInitialAdminUser();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
