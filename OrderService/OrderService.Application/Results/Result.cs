namespace OrderService.Application.Results;

public record Result<T> (
    T? Data,
    bool IsSuccess,
    string ErrorMessage
)
{
    public static Result<T> Success(T data) => new(data, true, string.Empty);
    public static Result<T> Failure(string errorMessage) => new(default, false, errorMessage);   
};

public record Result(
    bool IsSuccess,
    string ErrorMessage
)
{
    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string errorMessage) => new(false, errorMessage);   
};