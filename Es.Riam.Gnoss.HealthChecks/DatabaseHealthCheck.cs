using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.HealthChecks
{
    internal sealed class DatabaseHealthCheck<TContext> : IHealthCheck
        where TContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;

        // Acepta IServiceProvider en lugar de IDbContextFactory<TContext> para ser compatible
        // con servicios que registran el contexto con AddDbContext (scoped) y con los que usan
        // AddDbContextFactory — en ambos casos TContext se puede resolver desde un scope.
        public DatabaseHealthCheck(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                var ctx = scope.ServiceProvider.GetRequiredService<TContext>();
                var connection = ctx.Database.GetDbConnection();

                await connection.OpenAsync(cancellationToken);
                await connection.CloseAsync();

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, ex.Message);
            }
        }
    }
}
