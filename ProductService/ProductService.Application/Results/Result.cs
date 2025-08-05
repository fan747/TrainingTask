namespace ProductService.Application.Results;

public record Result<T> (
    T? Data,
    bool IsSuccess,
    ErrorType? ErrorType,
    string ErrorMessage
)
{
    public static Result<T> Success(T data) => new(data, true, null,string.Empty);
    public new static Result<T> Failure(ErrorType errorType, string errorMessage) => new(default, false,errorType, errorMessage);   
};

public enum ErrorType
{
    BadRequest,
    NotFound,
    InternalServerError
}

public record Result(
    bool IsSuccess,
    ErrorType? ErrorType,
    string ErrorMessage
)
{
    public static Result Success() => new(true, null, string.Empty);
    public static Result Failure(ErrorType errorType, string errorMessage) => new(false, errorType, errorMessage);   
};