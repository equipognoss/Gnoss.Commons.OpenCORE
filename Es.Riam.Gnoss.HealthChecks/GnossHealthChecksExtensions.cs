using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.HealthChecks
{
    public static class GnossHealthChecksExtensions
    {
        /// <summary>
        /// Añade el health check de base de datos según el proveedor configurado.
        /// dbProvider: "0" = SQL Server, "1" = Oracle, "2" = PostgreSQL
        /// </summary>
        public static IHealthChecksBuilder AddGnossDatabaseHealthCheck<TContext>(this IHealthChecksBuilder builder, string dbProvider, string connectionString)
            where TContext : DbContext
        {
            string name = dbProvider switch
            {
                "0" => "sqlserver",
                "1" => "oracle",
                _ => "postgresql"
            };

            return builder.Add(new HealthCheckRegistration(
                name,
                sp => new DatabaseHealthCheck<TContext>(sp),
                HealthStatus.Unhealthy,
                ["ready"],
                TimeSpan.FromSeconds(15)));
        }

        /// <summary>
        /// Añade el health check de Redis.
        /// </summary>
        public static IHealthChecksBuilder AddGnossRedisHealthCheck(this IHealthChecksBuilder builder, string redisConnectionString)
        {
            return builder.AddRedis(
                redisConnectionString: redisConnectionString,
                name: "redis",
                failureStatus: HealthStatus.Unhealthy,
                timeout: TimeSpan.FromSeconds(15),
                tags: ["ready"]);
        }

        /// <summary>
        /// Añade el health check de RabbitMQ con conexión persistente reutilizada.
        /// </summary>
        public static IHealthChecksBuilder AddGnossRabbitMQHealthCheck(this IHealthChecksBuilder builder, string? amqpUri)
        {
            if (string.IsNullOrEmpty(amqpUri))
                return builder;

            builder.Services.AddSingleton<RabbitMQHealthCheck>(_ => new RabbitMQHealthCheck(amqpUri));
            return builder.Add(new HealthCheckRegistration(
                "rabbitmq",
                sp => sp.GetRequiredService<RabbitMQHealthCheck>(),
                HealthStatus.Unhealthy,
                ["ready"],
                TimeSpan.FromSeconds(15)));
        }

        /// <summary>
        /// Añade el health check de Virtuoso ejecutando ASK {?s ?p ?o} sobre el endpoint SPARQL HTTP.
        /// </summary>
        public static IHealthChecksBuilder AddGnossVirtuosoHealthCheck(this IHealthChecksBuilder builder, string? connectionString, int sparqlPort = 8890)
        {
            if (string.IsNullOrEmpty(connectionString))
                return builder;

            builder.Services.AddSingleton<VirtuosoHealthCheck>(_ => new VirtuosoHealthCheck(connectionString, sparqlPort));
            return builder.Add(new HealthCheckRegistration(
                "virtuoso",
                sp => sp.GetRequiredService<VirtuosoHealthCheck>(),
                HealthStatus.Unhealthy,
                ["ready"],
                TimeSpan.FromSeconds(15)));
        }

        /// <summary>
        /// Mapea los endpoints /health/live y /health/ready en el puerto de management.
        /// Liveness no ejecuta checks — evita reinicios del contenedor por fallos de dependencias externas.
        /// Readiness evalúa todos los checks con tag "ready".
        /// IEndpointRouteBuilder es implementado tanto por WebApplication (patrón minimal API)
        /// como por el IEndpointRouteBuilder que recibe UseEndpoints() en el patrón Startup.cs.
        /// Esto permite llamar al método en ambos patrones sin duplicar código.
        /// </summary>
        public static void MapGnossHealthEndpoints(this IEndpointRouteBuilder endpoints, int managementPort)
        {
            var live = endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false,
                ResponseWriter = WriteHealthResponse
            });

            var ready = endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
                ResponseWriter = WriteHealthResponse
            });

#if !DEBUG
            live.RequireHost($"*:{managementPort}");
            ready.RequireHost($"*:{managementPort}");
#endif
        }

        private static Task WriteHealthResponse(HttpContext ctx, HealthReport report)
        {
            ctx.Response.ContentType = "application/json; charset=utf-8";

            string result = JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                duration = Math.Round(report.TotalDuration.TotalMilliseconds, 1),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    duration = Math.Round(e.Value.Duration.TotalMilliseconds, 1),
                    description = e.Value.Exception?.Message ?? e.Value.Description
                })
            }, new JsonSerializerOptions { WriteIndented = false });

            return ctx.Response.WriteAsync(result);
        }
    }
}
