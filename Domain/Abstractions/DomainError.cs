namespace Domain.Abstractions;

/// <summary>
/// Base record for domain errors.
/// </summary>
public abstract record DomainError(string Code, string Message);

