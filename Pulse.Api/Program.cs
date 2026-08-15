using Pulse.Api.Infrastructure;
using Pulse.Application.Features.Auth.Login;
using Pulse.Application.Features.Auth.Logout;
using Pulse.Application.Features.Auth.Refresh;
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

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName)
);

builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ITokenHasher, TokenHasher>();
builder.Services.AddScoped<RefreshService>();

builder.Services.AddScoped<LogoutService>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/");
}

app.MapPost("/auth/register", async (
    RegisterRequest request,
    RegisterService service,
    CancellationToken cancellationToken) =>
{
    var user = await service.RegisterAsync(request, cancellationToken);

    return Results.Ok(new
    {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    });
});

app.MapPost("/auth/login", async (
    LoginRequest request,
    LoginService service,
    CancellationToken cancellationToken) =>
{
    var response = await service.LoginAsync(request, cancellationToken);

    return Results.Ok(response);
});

app.MapPost("/auth/refresh", async (
    RefreshRequest request,
    RefreshService service,
    CancellationToken cancellationToken) =>
{
    var response = await service.RefreshAsync(request, cancellationToken);

    return Results.Ok(response);
});

app.MapPost("/auth/logout", async (
    LogoutRequest request,
    LogoutService service,
    CancellationToken cancellationToken) =>
{
    await service.LogoutAsync(request, cancellationToken);

    return Results.NoContent();
});

app.UseHttpsRedirection();

app.Run();