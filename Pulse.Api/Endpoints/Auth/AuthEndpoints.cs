using Pulse.Application.Features.Auth.Login;
using Pulse.Application.Features.Auth.Logout;
using Pulse.Application.Features.Auth.Refresh;
using Pulse.Application.Features.Auth.Register;

namespace Pulse.Api.Endpoints.Auth
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/auth");

            group.MapPost("/register", Register);
            group.MapPost("/login", Login);
            group.MapPost("/refresh", Refresh);
            group.MapPost("/auth", Logout);
            return endpoints;
        }

        private static async Task<IResult> Register(
            RegisterRequest request,
            RegisterService service,
            CancellationToken cancellationToken)
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
        }

        private static async Task<IResult> Login(
            LoginRequest request,
            LoginService service,
            CancellationToken cancellationToken)
        {
            var response = await service.LoginAsync(request, cancellationToken);

            return Results.Ok(response);
        }

        private static async Task<IResult> Refresh(
            RefreshRequest request,
            RefreshService service,
            CancellationToken cancellationToken)
        {
            var response = await service.RefreshAsync(request, cancellationToken);

            return Results.Ok(response);
        }

        private static async Task<IResult> Logout(
            LogoutRequest request,
            LogoutService service,
            CancellationToken cancellationToken)
        {
            await service.LogoutAsync(request, cancellationToken);

            return Results.NoContent();
        }
    }
}
