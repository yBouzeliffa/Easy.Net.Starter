using System.Reflection;

namespace Easy.Net.Starter.Extensions
{
    public class AppExtensions
    {
        public static string GetApplicationName()
        {
            return Assembly.GetEntryAssembly()?.GetName()?.Name ?? "UnknownApp";
        }
    }
}
