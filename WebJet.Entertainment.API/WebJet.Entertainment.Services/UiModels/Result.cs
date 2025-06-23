namespace WebJet.Entertainment.Services.UiModels;

public abstract record Result<T>;

// For when there is no data to return
public sealed record Unit;

public sealed record ErrorResult<T> : Result<T>
{
    public int ErrorCode { get; init; }
    public string ErrorMessage { get; init; }

    public Exception Exception { get; init; }
}


public sealed record SuccessResult<T> : Result<T>
{
    public T Data { get; init; }
}
