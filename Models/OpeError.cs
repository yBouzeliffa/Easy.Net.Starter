namespace Easy.Net.Starter.Models
{
    public class OpeError
    {
        public Exception Exception { get; set; } = null!;

        public string Message { get; set; } = null!;

        public Dictionary<string, string> Metadatas { get; set; } = new Dictionary<string, string>();
    }
}
