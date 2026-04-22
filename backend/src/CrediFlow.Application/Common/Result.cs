namespace CrediFlow.Application.Common;

public record Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Error { get; init; }
    public IEnumerable<string> ValidationErrors { get; init; } = Enumerable.Empty<string>();

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
    public static Result<T> ValidationFailure(IEnumerable<string> errors) =>
        new() { IsSuccess = false, ValidationErrors = errors };
}

public record Result
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    public IEnumerable<string> ValidationErrors { get; init; } = Enumerable.Empty<string>();

    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string error) => new() { IsSuccess = false, Error = error };
    public static Result ValidationFailure(IEnumerable<string> errors) =>
        new() { IsSuccess = false, ValidationErrors = errors };
}

public record PagedResult<T>
{
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
    public int TotalItems { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
