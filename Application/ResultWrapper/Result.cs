using Domain.enums;

namespace Application.ResultWrapper
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; }
        public T Value { get; }

        public ErrorType ErrorType { get; }

        private Result(bool isSuccess, T value, string? error = null ,ErrorType errorType=ErrorType.None)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            ErrorType = errorType;
        }

        public static Result<T> Success(T value) =>
            new(true, value, null);

        public static Result<T> FailureStatusCode(string error, ErrorType errorType) =>
            new(false, default!, error, errorType);

        public Result<T> OnSuccess(Action<T> action)
        {
            if (IsSuccess)
                action(Value);
            return this;
        }

        public Result<T> OnFailure(Action<string> action)
        {
            if (IsFailure)
                action(Error ?? string.Empty);

            return this;
        }
    }

}
