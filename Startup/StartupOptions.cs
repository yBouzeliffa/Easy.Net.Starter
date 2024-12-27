namespace Easy.Net.Starter.Startup
{
    public class StartupOptions
    {
        public bool UseJwtAuthentication { get; set; }
        public bool UseHttpLoggerMiddleware { get; set; }
        public bool UseSignalR { get; set; }
        public bool UseDatabase { get; set; }
        public Type? DatabaseContextType { get; set; }
        public string[] Middlewares { get; set; } = Array.Empty<string>();
        public string[] SingletonServices { get; set; } = Array.Empty<string>();
        public string[] ScopedServices { get; set; } = Array.Empty<string>();
        public string[] TransientServices { get; set; } = Array.Empty<string>();
        public IDictionary<string, string> SingletonsWithInterfaces { get; set; } = new Dictionary<string, string>();
        public IDictionary<string, string> ScopedWithInterfaces { get; set; } = new Dictionary<string, string>();
        public IDictionary<string, string> TransientsWithInterfaces { get; set; } = new Dictionary<string, string>();
    }
}
