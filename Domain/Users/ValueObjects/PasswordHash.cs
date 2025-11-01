namespace Domain.Users.ValueObjects;

/// <summary>
/// Value object representing a hashed password.
/// </summary>
public sealed record PasswordHash
{
    private PasswordHash(string value)
    {
        Value = value;
    }

    public string Value { get; }

    /// <summary>
    /// Creates a PasswordHash from a hashed string.
    /// This should only be called by the infrastructure layer after hashing.
    /// </summary>
    /// <param name="hashedPassword">The hashed password string.</param>
    /// <returns>A PasswordHash instance.</returns>
    public static PasswordHash Create(string hashedPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);
        return new PasswordHash(hashedPassword);
    }

    public override string ToString() => "***REDACTED***"; // Never expose hash in logs

    public static implicit operator string(PasswordHash hash) => hash.Value;
}

