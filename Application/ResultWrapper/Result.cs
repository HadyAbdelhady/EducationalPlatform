namespace Application.ResultWrapper
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; }
        public T Value { get; }

        private Result(bool isSuccess, T value, string? error = null)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) =>
            new(true, value, null);

        public static Result<T> Failure(string error) =>
            new(false, default!, error);

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
