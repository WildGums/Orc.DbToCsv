namespace Orc.DbToCsv
{
    using System;
    using System.IO;
    using System.Linq;
    using Catel.Logging;

    /// <summary>
    /// Manages backup files for the DbToCsv process.
    /// </summary>
    internal static class BackupManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates a backup of the specified file according to project backup settings.
        /// By default, backup files are always deleted after creation unless explicitly configured to be kept.
        /// </summary>
        /// <param name="originalFilePath">Path to the original file</param>
        /// <param name="project">The project containing backup configuration</param>
        /// <returns>The path to the created backup file, or null if no backup was created</returns>
        public static string? CreateBackup(string originalFilePath, Project project)
        {
            // Don't attempt to backup non-existent files
            if (!File.Exists(originalFilePath))
            {
                return null;
            }

            try
            {
                // Determine backup location
                string backupDirectory = GetBackupDirectory(originalFilePath, project);
                
                // Ensure the backup directory exists
                if (!Directory.Exists(backupDirectory))
                {
                    Directory.CreateDirectory(backupDirectory);
                }

                // Generate backup filename based on format
                string formatString = project.BackupFormat.Value ?? "{filename}_backup_{timestamp}{extension}";
                string backupFileName = GenerateBackupFileName(originalFilePath, formatString);
                string backupFilePath = Path.Combine(backupDirectory, backupFileName);

                // Copy the original file to the backup location
                File.Copy(originalFilePath, backupFilePath, true);
                
                // Clean up old backups if needed
                CleanupOldBackups(originalFilePath, project);
                
                return backupFilePath;
            }
            catch (Exception ex)
            {
                Log.Warning($"Failed to create backup for '{originalFilePath}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the directory where backups should be stored.
        /// </summary>
        private static string GetBackupDirectory(string originalFilePath, Project project)
        {
            if (!string.IsNullOrEmpty(project.BackupLocation.Value))
            {
                string backupLocation = project.BackupLocation.Value ?? string.Empty;
                
                // If the backup location is not rooted, make it relative to the output folder
                if (!string.IsNullOrEmpty(backupLocation) && !Path.IsPathRooted(backupLocation))
                {
                    string originalDirectory = Path.GetDirectoryName(originalFilePath) ?? string.Empty;
                    backupLocation = Path.Combine(originalDirectory, backupLocation);
                }
                
                return backupLocation;
            }
            
            // Default: store backups in the same directory as the original file
            return Path.GetDirectoryName(originalFilePath) ?? string.Empty;
        }

        /// <summary>
        /// Generates a backup filename based on the specified format.
        /// </summary>
        private static string GenerateBackupFileName(string originalFilePath, string format)
        {
            string fileName = Path.GetFileNameWithoutExtension(originalFilePath);
            string extension = Path.GetExtension(originalFilePath);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            
            return format
                .Replace("{filename}", fileName)
                .Replace("{extension}", extension)
                .Replace("{timestamp}", timestamp);
        }

        /// <summary>
        /// Cleans up old backup files based on the BackupFileCount setting.
        /// By default (BackupFileCount ≤ 0), all backup files are deleted.
        /// </summary>
        private static void CleanupOldBackups(string originalFilePath, Project project)
        {
            int maxBackups = project.BackupFileCount.Value;
            
            try
            {
                // Use the same directory calculation logic for both creating and cleaning up backups
                string backupDirectory = GetBackupDirectory(originalFilePath, project);
                
                // Log the backup directory being used
                Log.Debug($"Managing backups for '{Path.GetFileName(originalFilePath)}' in directory: {backupDirectory}");
                
                // Pattern to match backups for this specific file
                string fileName = Path.GetFileNameWithoutExtension(originalFilePath);
                string extension = Path.GetExtension(originalFilePath);
                string filePattern = $"{fileName}_backup_*{extension}";
                
                // Get all the matching backup files
                var allBackupFiles = Directory.GetFiles(backupDirectory, filePattern)
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime)  // Sort by creation time (newest first)
                    .Select(f => f.FullName)
                    .ToArray();
                
                // If BackupFileCount is 0 or negative, delete all backup files
                if (maxBackups <= 0)
                {
                    foreach (var file in allBackupFiles)
                    {
                        try
                        {
                            File.Delete(file);
                            Log.Debug($"Deleted backup file: {file}");
                        }
                        catch (Exception ex)
                        {
                            Log.Warning($"Failed to delete backup file '{file}': {ex.Message}");
                        }
                    }
                    return;
                }

                // If BackupFileCount > 0, keep that many newest backups and delete the rest
                var filesToDelete = allBackupFiles.Skip(maxBackups);
                foreach (var file in filesToDelete)
                {
                    try
                    {
                        File.Delete(file);
                        Log.Debug($"Deleted old backup file: {file}");
                    }
                    catch (Exception ex)
                    {
                        Log.Warning($"Failed to delete old backup file '{file}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"Failed to clean up old backups for '{originalFilePath}': {ex.Message}");
            }
        }
    }
}