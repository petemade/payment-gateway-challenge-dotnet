namespace PaymentGateway.Domain;

public class Result<TSuccess, TFailure>
{
    private readonly TSuccess _result;
    private readonly TFailure _failure;
    public bool IsSuccess { get; }

    public TSuccess Value => IsSuccess
        ? _result
        : throw new InvalidOperationException("Value is not accessible when unsuccessful");

    public TFailure Error => !IsSuccess
        ? _failure
        : throw new InvalidOperationException("Error is not accessible when successful");

    protected Result(TSuccess result, TFailure failure, bool isSuccess)
    {
        _result = result;
        _failure = failure;
        IsSuccess = isSuccess;
    }

    public static Result<TSuccess, TFailure> Success(TSuccess value) => new(value, default!, true);
    public static implicit operator Result<TSuccess, TFailure>(TSuccess value) => Success(value);

    public static Result<TSuccess, TFailure> Failure(TFailure error) => new(default!, error, false);
    public static implicit operator Result<TSuccess, TFailure>(TFailure error) => Failure(error);
}