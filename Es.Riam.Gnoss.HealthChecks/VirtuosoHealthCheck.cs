using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.HealthChecks
{
    // Singleton registrado en DI — reutiliza HttpClient durante toda la vida del servicio.
    // UID == "dba"  → endpoint /sparql sin credenciales HTTP (usuario dba accede al endpoint público).
    // UID != "dba"  → endpoint /sparql-auth con HTTP usando el usuario de lectura.
    internal sealed class VirtuosoHealthCheck : IHealthCheck, IDisposable
    {
        private const string AskQuery = "ASK {?s ?p ?o}";

        private readonly HttpClient _httpClient;
        private readonly string _endpoint;

        public VirtuosoHealthCheck(string connectionString, int sparqlPort = 8890)
        {
            VirtuosoConnectionInfo info = ParseConnectionString(connectionString);

            bool isDba = string.Equals(info.User, "dba", StringComparison.OrdinalIgnoreCase);
            _endpoint = isDba
                ? $"http://{info.Ip}:{sparqlPort}/sparql"
                : $"http://{info.Ip}:{sparqlPort}/sparql-auth";

            HttpClientHandler handler = new HttpClientHandler();
            if (!isDba && !string.IsNullOrEmpty(info.User))
            {
                handler.Credentials = new NetworkCredential(info.User, info.Password);
            }
                
            _httpClient = new HttpClient(handler);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("query", AskQuery),
                    new KeyValuePair<string, string>("format", "application/sparql-results+json")
                });

                using var response = await _httpClient.PostAsync(_endpoint, content, cancellationToken);

                return response.IsSuccessStatusCode
                    ? HealthCheckResult.Healthy()
                    : new HealthCheckResult(context.Registration.FailureStatus,
                        $"Virtuoso returned HTTP {(int)response.StatusCode}");
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, ex.Message);
            }
        }

        private static VirtuosoConnectionInfo ParseConnectionString(string connectionString)
        {
            string? ip = null, user = null, password = null;

            foreach (string part in connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                string[] kv = part.Split('=', 2, StringSplitOptions.TrimEntries);
                if (kv.Length != 2) continue;

                switch (kv[0].ToUpperInvariant())
                {
                    case "HOST":
                        string host = kv[1];
                        ip = host.Contains(':') ? host[..host.IndexOf(':')] : host;
                        break;
                    case "UID":
                        user = kv[1];
                        break;
                    case "PWD":
                        password = kv[1];
                        break;
                }
            }

            if (string.IsNullOrEmpty(ip))
            {
                throw new ArgumentException("HOST not found in Virtuoso connection string.", nameof(connectionString));
            }                

            return new VirtuosoConnectionInfo(ip, user, password);
        }

        public void Dispose() => _httpClient.Dispose();
    }
}
