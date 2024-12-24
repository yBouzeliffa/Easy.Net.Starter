using Microsoft.Extensions.Configuration;

namespace Easy.Net.Starter.App
{
    public static class ApplicationRegistrator
    {
        public static IConfiguration? RegisterConfiguration()
        {
            try
            {
                DirectoryInfo currentDirectories = new DirectoryInfo(Directory.GetCurrentDirectory());
                var appsettings = currentDirectories.GetFiles("appsettings.json", SearchOption.AllDirectories).FirstOrDefault();

                ArgumentException.ThrowIfNullOrEmpty(appsettings?.FullName, nameof(appsettings));

                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                    .AddJsonFile(appsettings?.FullName);

                return configurationBuilder.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to compile the configuration");
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
