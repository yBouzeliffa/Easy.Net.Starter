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
            string envVarName = $"DATABASE_{databaseName.ToUpper()}";

            string? password = Environment.GetEnvironmentVariable(envVarName, EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(password))
            {
                if (Environment.UserInteractive)
                {
                    password = AnsiConsole.Prompt(new TextPrompt<string>("[yellow]Database Password:[/]").Secret());

                    Environment.SetEnvironmentVariable(envVarName, password, EnvironmentVariableTarget.User);
                }
                else
                {
                    throw new InvalidOperationException($"La variable d'environnement {envVarName} n'est pas définie.");
                }
            }

            ArgumentException.ThrowIfNullOrEmpty(password, "Le mot de passe de la base de données n'est pas défini.");

            return password;
        }
    }
}