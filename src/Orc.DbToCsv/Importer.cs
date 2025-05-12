namespace Orc.DbToCsv
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Logging;
    using CsvHelper;
    using DataAccess.Database;
    using Microsoft.Data.SqlClient;

    /// <summary>
    /// Handles the export of database tables to CSV files.
    /// </summary>
    public static class Importer
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Processes a project file to export tables to CSV files.
        /// </summary>
        /// <param name="projectFilePath">Path to the project file.</param>
        /// <param name="outputFolderPath">Output folder path (optional, can be defined in project).</param>
        public static async Task ProcessProjectAsync(string projectFilePath, string outputFolderPath)
        {
            var project = await Project.LoadAsync(projectFilePath);
            if (project is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Failed to load project from path: {projectFilePath}");
            }
            
            await ProcessProjectAsync(project);
        }

        /// <summary>
        /// Processes a project to export tables to CSV files.
        /// </summary>
        /// <param name="project">The project to process.</param>
        public static async Task ProcessProjectAsync(Project project)
        {
            if (project is null)
            {
                throw Log.ErrorAndCreateException<ArgumentNullException>(nameof(project), "Project cannot be null");
            }

            Log.Info("Project processing started ...");

            try
            {
                var exportDescriptions = project.GetDbToCsvExportDescriptions();

                Log.Info("{0} tables to process", exportDescriptions.Count.ToString());

                foreach (var exportDescription in exportDescriptions)
                {
                    await ProcessTableAsync(exportDescription, project);
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex.Message);
                throw; // Rethrow to ensure error is propagated to caller
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw; // Rethrow to ensure error is propagated to caller
            }
        }

        /// <summary>
        /// Processes a single table export to CSV.
        /// </summary>
        private static async Task ProcessTableAsync(DbToCsvExportDescription exportDescription, Project project)
        {
            // Validate input parameters
            var (fullFileName, outputFolderPath, source) = ValidateExportParameters(exportDescription);

            string tempFileName = string.Empty;
            int records = 0;

            try
            {
                // Create a unique temporary file for writing data
                tempFileName = CreateTempFileName(fullFileName, outputFolderPath);
                
                // Export data from database to the temporary CSV file
                records = await ExportDataToCsvAsync(
                    tempFileName,
                    source,
                    project.MaximumRowsInTable.Value,
                    exportDescription.Parameters ?? new DataAccess.DataSourceParameters());
                
                // Only replace the original file if database export was successful
                ReplaceOriginalWithTemp(fullFileName, tempFileName, outputFolderPath, project);
                
                Log.Info($"{records} records of '{source.Schema}' '{source.Table}' table successfully exported to {fullFileName}.");
            }
            catch (Exception ex)
            {
                Log.Error($"{source.Table} export failed because of exception: {ex.Message}");
                
                // Clean up the temporary file we were writing to
                CleanupTempFile(tempFileName);
                
                // Rethrow all exceptions to ensure proper error propagation
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Failed to process table '{source.Table}'. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Validates export parameters and ensures output directory exists.
        /// </summary>
        private static (string fullFileName, string outputFolderPath, DatabaseSource source) ValidateExportParameters(
            DbToCsvExportDescription exportDescription)
        {
            var fullFileName = exportDescription.CsvFilePath;
            if (string.IsNullOrWhiteSpace(fullFileName))
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot process empty csv file path");
            }

            var outputFolderPath = Path.GetDirectoryName(fullFileName);
            if (string.IsNullOrWhiteSpace(outputFolderPath))
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot process empty output folder path");
            }

            var source = exportDescription.Source;
            if (source is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot process null source");
            }

            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            return (fullFileName, outputFolderPath, source);
        }

        /// <summary>
        /// Creates a unique temporary file name.
        /// </summary>
        private static string CreateTempFileName(string fullFileName, string outputFolderPath)
        {
            return Path.Combine(
                outputFolderPath,
                $"{Path.GetFileNameWithoutExtension(fullFileName)}_temp_{Guid.NewGuid():N}{Path.GetExtension(fullFileName)}");
        }

        /// <summary>
        /// Exports data from the database to a CSV file.
        /// </summary>
        private static async Task<int> ExportDataToCsvAsync(
            string csvFilePath,
            DatabaseSource source,
            int maximumRows,
            DataAccess.DataSourceParameters parameters)
        {
            int records = 0;

            await using var streamWriter = new StreamWriter(new FileStream(csvFilePath, FileMode.Create));
            await using var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture);
            
            using var dataReader = new SqlTableReader(source.ToString(), 0, maximumRows, parameters);
            
            while (true)
            {
                var headers = dataReader.FieldHeaders;
                if (dataReader.ValidationContext.HasErrors)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>($"Database validation error: {dataReader.ValidationContext}");
                }

                // Write headers if available
                if (headers.Any())
                {
                    foreach (var field in headers)
                    {
                        csvWriter.WriteField(field);
                    }
                    await csvWriter.NextRecordAsync();
                }

                // Write data rows
                while (await dataReader.ReadAsync())
                {
                    for (var i = 0; i < headers.Length; i++)
                    {
                        var value = dataReader.GetValue(i);

                        if (value is string strValue)
                        {
                            value = strValue.Trim();
                        }

                        csvWriter.WriteField(value);
                    }

                    records++;
                    await csvWriter.NextRecordAsync();
                }

                if (dataReader.ValidationContext.HasErrors)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>($"Database validation error: {dataReader.ValidationContext}");
                }

                // Check if there are more result sets
                if (!await dataReader.NextResultAsync())
                {
                    break;
                }
            }

            return records;
        }

        /// <summary>
        /// Creates a backup of the original file (if configured) and replaces it with the temp file.
        /// </summary>
        /// <exception cref="IOException">Thrown when file operations fail</exception>
        private static void ReplaceOriginalWithTemp(string originalFilePath, string tempFilePath, string outputFolderPath, Project project)
        {
            string? backupFilePath = null;
            bool originalFileExisted = File.Exists(originalFilePath);
            
            try
            {
                if (originalFileExisted)
                {
                    // Create a backup according to project settings, and store the backup path
                    backupFilePath = BackupManager.CreateBackup(originalFilePath, project);
                    
                    // Delete the original file after backup is created
                    File.Delete(originalFilePath);
                }

                try
                {
                    // Move the temp file to the final destination
                    File.Move(tempFilePath, originalFilePath);
                    
                    // Log successful file replacement
                    Log.Debug($"Successfully replaced original file with temporary file. Path: {originalFilePath}");
                }
                catch (Exception moveEx)
                {
                    // Attempt to restore the original file if we have a backup
                    if (originalFileExisted && !string.IsNullOrEmpty(backupFilePath) && File.Exists(backupFilePath))
                    {
                        try
                        {
                            // Copy the backup back to the original location
                            File.Copy(backupFilePath, originalFilePath, true);
                            Log.Warning($"Restored original file from backup after import failure: {originalFilePath}");
                        }
                        catch (Exception restoreEx)
                        {
                            Log.Error($"Failed to restore original file from backup: {restoreEx.Message}");
                        }
                    }
                    
                    // Clean up the temporary file
                    CleanupTempFile(tempFilePath);
                    
                    throw Log.ErrorAndCreateException<IOException>(
                        $"Failed to move temporary file to destination. Original file has been restored. Temp: {tempFilePath}, Destination: {originalFilePath}",
                        moveEx);
                }
            }
            catch (Exception ex)
            {
                // If the exception occurred after backup was created but before the original was deleted,
                // or in any unexpected case, try to restore from backup if it's available
                if (originalFileExisted && !File.Exists(originalFilePath) &&
                    !string.IsNullOrEmpty(backupFilePath) && File.Exists(backupFilePath))
                {
                    try
                    {
                        File.Copy(backupFilePath, originalFilePath, true);
                        Log.Warning($"Restored original file from backup after exception: {originalFilePath}");
                    }
                    catch (Exception restoreEx)
                    {
                        Log.Error($"Failed to restore original file from backup: {restoreEx.Message}");
                    }
                }
                
                // Always clean up the temporary file if it exists
                CleanupTempFile(tempFilePath);
                
                throw Log.ErrorAndCreateException<IOException>(
                    $"Failed to replace original file with temporary file. Original: {originalFilePath}, Temp: {tempFilePath}",
                    ex);
            }
        }

        /// <summary>
        /// Attempts to clean up a temporary file if it exists.
        /// </summary>
        private static void CleanupTempFile(string tempFileName)
        {
            if (!string.IsNullOrEmpty(tempFileName) && File.Exists(tempFileName))
            {
                try
                {
                    File.Delete(tempFileName);
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to delete temporary file: {ex.Message}");
                }
            }
        }
    }
}
