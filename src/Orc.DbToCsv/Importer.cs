// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Importer.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Methods
        public static async Task ProcessProjectAsync(string projectFilePath, string outputFolderPath)
        {
            var project = await Project.LoadAsync(projectFilePath);
            await ProcessProjectAsync(project);
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
            var outputFolderPath = Path.GetDirectoryName(fullFileName);
            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            var records = 0;
            var source = exportDescription.Source;

            try
            {
                if (File.Exists(fullFileName))
                {
                    File.Delete(fullFileName);
                }

                using (var streamWriter = new StreamWriter(new FileStream(fullFileName, FileMode.OpenOrCreate)))
                {
                    using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture))
                    {
                        using (var dataReader = new SqlTableReader(source.ToString(), 0, project.MaximumRowsInTable.Value, exportDescription.Parameters))
                        {
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
                        }
                    }
                }

                Log.Info($"{records} records of '{source.Schema}' '{source.Table}' table successfully exported to {fullFileName}.");
            }
            catch (Exception ex)
            {
                Log.Error($"{source.Table} export failed because of exception: {ex.Message}");
            }
        }
        #endregion
    }
}
