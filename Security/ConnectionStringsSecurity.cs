using Spectre.Console;

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

            string? password = Environment.GetEnvironmentVariable(envVarName);

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

                    Environment.SetEnvironmentVariable(envVarName, password);
                }
            }

            ArgumentException.ThrowIfNullOrEmpty(password, "Le mot de passe de la base de données n'est pas défini.");

            return password;
        }
    }
}