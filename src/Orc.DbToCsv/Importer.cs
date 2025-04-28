namespace Orc.DbToCsv
{
    using System;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Logging;
    using CsvHelper;
    using DataAccess.Database;

    public static class Importer
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static async Task ProcessProjectAsync(string projectFilePath, string outputFolderPath)
        {
            var project = await Project.LoadAsync(projectFilePath);
            if (project is not null)
            {
                await ProcessProjectAsync(project);
            }
        }

        public static async Task ProcessProjectAsync(Project project)
        {
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
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static async Task ProcessTableAsync(DbToCsvExportDescription exportDescription, Project project)
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

            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            var records = 0;
            var source = exportDescription.Source;
            if (source is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot process null source");
            }

            try
            {
                // Create a temporary file for writing data
                var tempFileName = Path.Combine(
                    outputFolderPath,
                    $"{Path.GetFileNameWithoutExtension(fullFileName)}_temp_{Guid.NewGuid():N}{Path.GetExtension(fullFileName)}");

                await using var streamWriter = new StreamWriter(new FileStream(tempFileName, FileMode.Create));
                await using var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture);
                using var dataReader = new SqlTableReader(source.ToString(), 0, project.MaximumRowsInTable.Value, exportDescription.Parameters);
                while (true)
                {
                    var headers = dataReader.FieldHeaders;
                    if (dataReader.ValidationContext.HasErrors)
                    {
                        Log.Error(dataReader.ValidationContext.ToString());

                        return;
                    }

                    if (headers.Any())
                    {
                        foreach (var field in headers)
                        {
                            csvWriter.WriteField(field);
                        }

                        await csvWriter.NextRecordAsync();
                    }

                    while (await dataReader.ReadAsync())
                    {
                        for (var i = 0; i < headers.Length; i++)
                        {
                            var value = dataReader.GetValue(i);

                            if (value is string strValue)
                            {
                                value = strValue.Trim(); // Remove all white spaces
                            }

                            csvWriter.WriteField(value);
                        }

                        records++;

                        await csvWriter.NextRecordAsync();
                    }

                    if (dataReader.ValidationContext.HasErrors)
                    {
                        Log.Error(dataReader.ValidationContext.ToString());

                        return;
                    }

                    if (!await dataReader.NextResultAsync())
                    {
                        break;
                    }
                }

                // Only replace the original file if we successfully read from the database
                if (File.Exists(fullFileName))
                {
                    // Create a backup of the original file just in case
                    var backupFileName = Path.Combine(
                        outputFolderPath,
                        $"{Path.GetFileNameWithoutExtension(fullFileName)}_backup_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fullFileName)}");
                    
                    File.Copy(fullFileName, backupFileName, true);
                    File.Delete(fullFileName);
                }

                // Move the temp file to the final destination
                File.Move(tempFileName, fullFileName);
                
                Log.Info($"{records} records of '{source.Schema}' '{source.Table}' table successfully exported to {fullFileName}.");
            }
            catch (Exception ex)
            {
                Log.Error($"{source.Table} export failed because of exception: {ex.Message}");
                
                // Clean up any temporary file if it exists
                var tempFileName = Path.Combine(
                    outputFolderPath,
                    $"{Path.GetFileNameWithoutExtension(fullFileName)}_temp_{Guid.NewGuid():N}{Path.GetExtension(fullFileName)}");
                
                if (File.Exists(tempFileName))
                {
                    try
                    {
                        File.Delete(tempFileName);
                    }
                    catch (Exception deleteEx)
                    {
                        Log.Error($"Failed to delete temporary file: {deleteEx.Message}");
                    }
                }
            }
        }
    }
}
