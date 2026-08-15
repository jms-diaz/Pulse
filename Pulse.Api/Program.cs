using Pulse.Api.Endpoints.Auth;
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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddOpenApi();

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("Admin");
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();

app.MapGet("/test-auth", (
    ICurrentUser currentUser) =>
{
    return Results.Ok(new
    {
        currentUser.UserId,
        currentUser.Email,
        currentUser.IsAuthenticated
    });
})
.RequireAuthorization();

app.MapGet("/admin-test", () =>
{
    return Results.Ok("You are an admin.");
})
.RequireAuthorization("AdminOnly");

app.MapGet("/debug-auth", (HttpContext context) =>
{
    return Results.Ok(new
    {
        IsAuthenticated = context.User.Identity?.IsAuthenticated,
        AuthenticationType = context.User.Identity?.AuthenticationType,
        Claims = context.User.Claims.Select(c => new
        {
            c.Type,
            c.Value
        })
    });
});

app.Run();