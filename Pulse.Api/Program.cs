using Pulse.Api.Infrastructure;
using Pulse.Application.Features.Auth.Register;
using Pulse.Application.Interfaces;
using Pulse.Infrastructure.Authentication;
using Pulse.Infrastructure.Persistence;
using Pulse.Infrastructure.Persistence.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDataSource("pulse");
builder.AddNpgsqlDbContext<AppDbContext>("pulse");

builder.AddRedisDistributedCache("redis");

builder.Services.AddHybridCache();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHostedService<DatabaseInitializer>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<RegisterService>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapPost("/auth/register", async (
    RegisterRequest request,
    RegisterService service) =>
{
    var user = await service.RegisterAsync(request);

    return Results.Ok(new
    {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    });
});

app.UseHttpsRedirection();

app.Run();