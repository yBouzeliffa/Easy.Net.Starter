namespace Easy.Net.Starter.Models
{
    public class OpeResult<T> : Result<T, OpeError>
    {
        public OpeResult(bool isSuccess, T? value, OpeError? error) : base(isSuccess, value, error)
        {
        }

        public OpeResult<T> Tag(string key, string tag)
        {
            if (IsSuccess is false)
                Error.Metadatas[key] = tag;

            return this;
        }

        public static new OpeResult<T> Success(T value) => new(true, value, default);
        public static OpeResult<T> Failure(Exception ex, string message) => new(false, default, new(ex, message));
    }
}
