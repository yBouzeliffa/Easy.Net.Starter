using System.Net;

namespace Easy.Net.Starter.Models
{
    public class Result<T, TError>
    {
        private T? _value;
        private TError? _error;

        public bool IsSuccess { get; }

        public HttpStatusCode HttpCode = HttpStatusCode.OK;

        public T Value
        {
            get => IsSuccess ? _value! : throw new InvalidOperationException("Result is not successful");
            private set => _value = value;
        }

        public TError Error
        {
            get => !IsSuccess ? _error! : throw new InvalidOperationException("Result is successful");
            private set => _error = value;
        }

        protected Result(bool isSuccess, T? value, TError? error) => (IsSuccess, _value, _error) = (isSuccess, value, error);

        public Result<T, TError> WithHttpStatusCode(HttpStatusCode httpStatusCode)
        {
            HttpCode = httpStatusCode;
            return this;
        }

        public static Result<T, TError> Success(T value) => new(true, value, default);
        public static Result<T, TError> Failure(TError error) => new(false, default, error);
    }
}
