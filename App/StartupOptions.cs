namespace Easy.Net.Starter.App
{
    public class StartupOptions
    {
        public bool WithDatabase { get; set; }
        public string[] Middlewares { get; set; } = Array.Empty<string>();
        public string[] SingletonServices { get; set; } = Array.Empty<string>();
        public string[] ScopedServices { get; set; } = Array.Empty<string>();
        public string[] TransientServices { get; set; } = Array.Empty<string>();
        public IDictionary<string, string> SingletonsWithInterfaces { get; set; } = new Dictionary<string, string>();
        public IDictionary<string, string> ScopedWithInterfaces { get; set; } = new Dictionary<string, string>();
        public IDictionary<string, string> TransientsWithInterfaces { get; set; } = new Dictionary<string, string>();
    }

}
