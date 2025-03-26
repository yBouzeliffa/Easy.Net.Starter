using System.Net;

namespace Easy.Net.Starter.Models
{
    public class Result<T, TError>
    {
        private T? _value;
        private TError? _error;
        private HttpStatusCode _httpCode = HttpStatusCode.OK;

        public bool IsSuccess { get; }


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

        public HttpStatusCode GetHttpStatusCode() => IsSuccess ? HttpStatusCode.OK : _httpCode;

        protected Result(bool isSuccess, T? value, TError? error) => (IsSuccess, _value, _error) = (isSuccess, value, error);
        protected Result(bool isSuccess, T? value, TError? error, HttpStatusCode httpStatusCode) => (IsSuccess, _value, _error, _httpCode) = (isSuccess, value, error, httpStatusCode);

        public static Result<T, TError> Success(T value) => new(true, value, default);
        public static Result<T, TError> Failure(TError error, HttpStatusCode httpStatusCode = HttpStatusCode.OK) => new(false, default, error, httpStatusCode);
    }
}
