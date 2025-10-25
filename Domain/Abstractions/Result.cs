namespace Domain.Abstractions;

/// <summary>
/// Represents the result of an operation that can succeed or fail.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error.</typeparam>
public class Result<TValue, TError>
{
    private readonly TValue? _value;
    private readonly TError? _error;

    private Result(TValue value)
    {
        IsSuccess = true;
        _value = value;
        _error = default;
    }

    private Result(TError error)
    {
        IsSuccess = false;
        _value = default;
        _error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value when Result is a failure.");

    public TError Error => IsFailure
        ? _error!
        : throw new InvalidOperationException("Cannot access Error when Result is a success.");

    public static Result<TValue, TError> Success(TValue value) => new(value);
    public static Result<TValue, TError> Failure(TError error) => new(error);

    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess,
        Func<TError, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(_value!) : onFailure(_error!);
    }
}

/// <summary>
/// Represents the result of an operation that can succeed or fail without a return value.
/// </summary>
/// <typeparam name="TError">The type of the error.</typeparam>
public class Result<TError>
{
    private readonly TError? _error;

    private Result(bool isSuccess, TError? error)
    {
        IsSuccess = isSuccess;
        _error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public TError Error => IsFailure
        ? _error!
        : throw new InvalidOperationException("Cannot access Error when Result is a success.");

    public static Result<TError> Success() => new(true, default);
    public static Result<TError> Failure(TError error) => new(false, error);

    public TResult Match<TResult>(
        Func<TResult> onSuccess,
        Func<TError, TResult> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(_error!);
    }
}

