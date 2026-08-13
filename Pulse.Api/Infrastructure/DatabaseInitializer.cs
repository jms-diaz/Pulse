using Microsoft.EntityFrameworkCore;
using Pulse.Infrastructure.Persistence;

namespace Pulse.Api.Infrastructure
{
    public sealed class DatabaseInitializer(
    IServiceScopeFactory scopeFactory,
    ILogger<DatabaseInitializer> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                await dbContext.Database.MigrateAsync(cancellationToken);

                logger.LogInformation(
                    "Database migration completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error applying database migrations.");

                throw;
            }
        }
    }
}
