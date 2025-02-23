using Spectre.Console;
using System.Runtime.InteropServices;

namespace Easy.Net.Starter.Security
{
    public static class ConnectionStringsSecurityManager
    {
        public static string UpdateConnectionPassword(this string connectionString, string databaseName)
        {
            string password = GetDatabasePassword(databaseName);
            return connectionString.Replace("{{DATABASE_PASSWORD}}", password);
        }

        public static string GetDatabasePassword(string databaseName)
        {
            string envVarName = $"DATABASE_{databaseName.Replace("-", "_").ToUpper()}";

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            string? password = isWindows
                ? Environment.GetEnvironmentVariable(envVarName, EnvironmentVariableTarget.User)
                : Environment.GetEnvironmentVariable(envVarName);

            if (string.IsNullOrEmpty(password))
            {
                if (Console.IsInputRedirected)
                {
                    throw new InvalidOperationException(
                        $"La variable d'environnement {envVarName} n'est pas définie et l'entrée interactive n'est pas disponible.");
                }
                else
                {
                    password = AnsiConsole.Prompt(
                        new TextPrompt<string>("[yellow]Database Password:[/]")
                            .Secret());

                    if (isWindows)
                    {
                        Environment.SetEnvironmentVariable(envVarName, password, EnvironmentVariableTarget.User);
                    }
                    else
                    {
                        Environment.SetEnvironmentVariable(envVarName, password);
                    }
                }
            }

            ArgumentException.ThrowIfNullOrEmpty(password, "Le mot de passe de la base de données n'est pas défini.");

            return password;
        }
    }
}