namespace Easy.Net.Starter.PostgreSQL
{
    public static class PgDumpFinder
    {
        public static string FindPgDumpPath()
        {
            List<string> baseDirectories = [];

            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            if (!string.IsNullOrEmpty(programFiles))
                baseDirectories.Add(Path.Combine(programFiles, "PostgreSQL"));
            if (!string.IsNullOrEmpty(programFilesX86))
                baseDirectories.Add(Path.Combine(programFilesX86, "PostgreSQL"));

            foreach (string baseDir in baseDirectories)
            {
                if (Directory.Exists(baseDir))
                {
                    try
                    {
                        string[] files = Directory.GetFiles(baseDir, "pg_dump.exe", SearchOption.AllDirectories);
                        if (files != null && files.Length > 0)
                        {
                            return files[0];
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine($"Accès refusé à {baseDir}: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur lors de la recherche dans {baseDir}: {ex.Message}");
                    }
                }
            }

            throw new ArgumentNullException("pg_dump.exe n'a pas été trouvé.");
        }
    }
}