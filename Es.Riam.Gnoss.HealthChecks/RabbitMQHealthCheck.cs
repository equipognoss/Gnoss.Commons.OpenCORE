using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.HealthChecks
{
    // Singleton registrado en DI — mantiene una IConnection reutilizable para los checks.
    internal sealed class RabbitMQHealthCheck : IHealthCheck, IDisposable
    {
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private readonly object _lock = new();

        public RabbitMQHealthCheck(string amqpUri)
        {
            _factory = new ConnectionFactory { Uri = new Uri(amqpUri) };
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                lock (_lock)
                {
                    if (_connection is null || !_connection.IsOpen)
                    {
                        _connection?.Dispose();
                        _connection = _factory.CreateConnection();
                    }
                }

                return Task.FromResult(_connection.IsOpen
                    ? HealthCheckResult.Healthy()
                    : new HealthCheckResult(context.Registration.FailureStatus, "RabbitMQ connection is not open"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, ex.Message));
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _connection?.Dispose();
                _connection = null;
            }
        }
    }
}
