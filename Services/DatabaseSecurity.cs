using System.Diagnostics;

public static class DatabaseSecurity
{
    public static string GetDatabasePassword()
    {
        string appName = Process.GetCurrentProcess().ProcessName;
        if (string.IsNullOrEmpty(appName))
        {
            throw new InvalidOperationException("Impossible de déterminer le nom de l'application.");
        }

        string envVarName = $"DATABASE_{appName.ToUpper()}";

        string password = Environment.GetEnvironmentVariable(envVarName);

        if (string.IsNullOrEmpty(password))
        {
            if (Environment.UserInteractive)
            {
                Console.Write("Veuillez saisir le mot de passe de la base de données : ");
                password = Console.ReadLine();

                Environment.SetEnvironmentVariable(envVarName, password, EnvironmentVariableTarget.Process);
            }
            else
            {
                throw new InvalidOperationException($"La variable d'environnement {envVarName} n'est pas définie.");
            }
        }

        return password;
    }
}
