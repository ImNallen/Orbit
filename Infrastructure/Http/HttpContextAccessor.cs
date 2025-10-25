using Domain.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Http;

/// <summary>
/// Implementation of IHttpContextAccessor that retrieves information from the current HTTP context.
/// </summary>
public sealed class HttpContextAccessor : Domain.Abstractions.IHttpContextAccessor
{
    private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;

    public HttpContextAccessor(Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the IP address of the current request.
    /// Returns "Unknown" if the IP address cannot be determined.
    /// </summary>
    public string IpAddress
    {
        get
        {
            HttpContext? context = _httpContextAccessor.HttpContext;
            if (context is null)
            {
                return "Unknown";
            }

            // Try to get the real IP address from X-Forwarded-For header (for proxies/load balancers)
            string? forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                // X-Forwarded-For can contain multiple IPs, take the first one
                return forwardedFor.Split(',')[0].Trim();
            }

            // Fall back to RemoteIpAddress
            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }

    /// <summary>
    /// Gets the User-Agent header of the current request.
    /// Returns "Unknown" if the User-Agent cannot be determined.
    /// </summary>
    public string UserAgent
    {
        get
        {
            HttpContext? context = _httpContextAccessor.HttpContext;
            if (context is null)
            {
                return "Unknown";
            }

            return context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
        }
    }
}

