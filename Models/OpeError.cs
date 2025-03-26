namespace Easy.Net.Starter.Models
{
    public class OpeError
    {
        public Exception Exception { get; set; } = null!;

        public string Message { get; set; } = null!;

        public OpeError(Exception exception, string message)
        {
            Exception = exception;
            Message = message;
        }

        public Dictionary<string, string> Metadatas { get; set; } = [];
    }
}
