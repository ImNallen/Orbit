using System.Security.Claims;
using HotChocolate.Authorization;

namespace Api.GraphQL.Queries;

/// <summary>
/// GraphQL queries for health checks.
/// </summary>
[ExtendObjectType("Query")]
public sealed class HealthQueries
{
    /// <summary>
    /// Simple health check query.
    /// </summary>
    public string Health => "OK";
}
