using System.Diagnostics;
using System.Reflection;
using Api.GraphQL.Types;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Api.GraphQL.Queries;

/// <summary>
/// GraphQL queries for health checks.
/// </summary>
[ExtendObjectType("Query")]
public sealed class HealthQueries
{
    private static readonly DateTime _startTime = DateTime.UtcNow;

    /// <summary>
    /// Comprehensive health check query that verifies database connectivity and system status.
    /// </summary>
    public async Task<HealthType> HealthAsync(
        [Service] ApplicationDbContext dbContext,
        [Service] IWebHostEnvironment environment,
        CancellationToken cancellationToken = default)
    {
        DateTime timestamp = DateTime.UtcNow;
        double uptime = (timestamp - _startTime).TotalSeconds;

        // Check database connectivity
        string databaseStatus;
        string overallStatus;
        string? details = null;

        try
        {
            // Attempt to connect to the database
            bool canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

            if (canConnect)
            {
                databaseStatus = "Connected";
                overallStatus = "Healthy";
            }
            else
            {
                databaseStatus = "Disconnected";
                overallStatus = "Unhealthy";
                details = "Database connection failed";
            }
        }
        catch (Exception ex)
        {
            databaseStatus = "Error";
            overallStatus = "Unhealthy";
            details = $"Database error: {ex.Message}";
        }

        // Get application version
        string version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "Unknown";

        return new HealthType
        {
            Status = overallStatus,
            Timestamp = timestamp,
            Environment = environment.EnvironmentName,
            Database = databaseStatus,
            Version = version,
            UptimeSeconds = uptime,
            Details = details
        };
    }
}
