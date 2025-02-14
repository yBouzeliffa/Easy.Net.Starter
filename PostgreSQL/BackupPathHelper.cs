namespace Easy.Net.Starter.PostgreSQL
{
    public static class BackupPathHelper
    {
        public static void EnsureDirectoryExistsForFile(string backupFilePath)
        {
            if (string.IsNullOrWhiteSpace(backupFilePath))
            {
                throw new ArgumentException("Le chemin du fichier de sauvegarde ne peut être nul ou vide.", nameof(backupFilePath));
            }

            string directoryPath = Path.GetDirectoryName(backupFilePath);

            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentException("Le chemin spécifié n'est pas valide.", nameof(backupFilePath));
            }

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
