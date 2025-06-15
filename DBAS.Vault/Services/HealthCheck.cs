using DBAS.Vault.Database;
using DBAS.Vault.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DBAS.Vault.Services
{
    public class HealthCheck : IHealthCheck
    {
        private readonly APIEnvironment Environment;

        public HealthCheck(APIEnvironment env)
        {
            Environment = env;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using var db = new MySqlDb(Environment);

            if (db.IsHealthy())
            {
                return await Task.FromResult(HealthCheckResult.Healthy(nameof(HealthCheck)));
            }
            else
            {
                return await Task.FromResult(HealthCheckResult.Unhealthy(nameof(HealthCheck)));
            }
        }
    }
}