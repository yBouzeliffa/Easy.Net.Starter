using Easy.Net.Starter.Extensions;
using Spectre.Console;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Easy.Net.Starter.Security
{
    public enum EmailApiKey
    {
        [Description("SENDGRID")]
        SendGrid
    }

    public class EmailApiKeySecurity
    {
        public static string GetApiKey(EmailApiKey emailApiKey)
        {
            string envVarName = $"{emailApiKey.GetDescription()}_{AppExtensions.GetApplicationName().ToUpper()}";

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            string? apiKey = isWindows
                ? Environment.GetEnvironmentVariable(envVarName, EnvironmentVariableTarget.User)
                : Environment.GetEnvironmentVariable(envVarName);

            if (string.IsNullOrEmpty(apiKey))
            {
                if (Console.IsInputRedirected)
                {
                    throw new InvalidOperationException(
                        $"The environment variable {envVarName} is not defined and interactive input is not available.");
                }
                else
                {
                    apiKey = AnsiConsole.Prompt(
                        new TextPrompt<string>($"[yellow]{emailApiKey.GetDescription()} Api Key:[/]")
                            .Secret());

                    if (isWindows)
                    {
                        Environment.SetEnvironmentVariable(envVarName, apiKey, EnvironmentVariableTarget.User);
                    }
                    else
                    {
                        Environment.SetEnvironmentVariable(envVarName, apiKey);
                    }
                }
            }

            ArgumentException.ThrowIfNullOrEmpty(apiKey, "The api key password is not set.");

            return apiKey;
        }
    }
}
