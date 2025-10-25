namespace Domain.Abstractions;

/// <summary>
/// Abstraction for accessing HTTP context information.
/// </summary>
public interface IHttpContextAccessor
{
    /// <summary>
    /// Gets the IP address of the current request.
    /// </summary>
    string IpAddress { get; }

    /// <summary>
    /// Gets the User-Agent header of the current request.
    /// </summary>
    string UserAgent { get; }
}

