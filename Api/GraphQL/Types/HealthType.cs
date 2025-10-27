namespace Api.GraphQL.Types;

/// <summary>
/// GraphQL type for health check information.
/// </summary>
public sealed class HealthType
{
    /// <summary>
    /// Overall health status of the application.
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Timestamp when the health check was performed.
    /// </summary>
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// Current environment (Development, Staging, Production).
    /// </summary>
    public string Environment { get; init; } = string.Empty;

    /// <summary>
    /// Database connectivity status.
    /// </summary>
    public string Database { get; init; } = string.Empty;

    /// <summary>
    /// Application version.
    /// </summary>
    public string? Version { get; init; }

    /// <summary>
    /// Application uptime in seconds.
    /// </summary>
    public double UptimeSeconds { get; init; }

    /// <summary>
    /// Additional details about the health check.
    /// </summary>
    public string? Details { get; init; }
}

