// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Importer.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Collections;
    using Catel.Logging;
    using CsvHelper;
    using DatabaseManagement;

    public static class Importer
    {
        #region Constants
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

                //var recors = source.CreateGateway().GetRecords().ReadAll();
                var objects = source.CreateGateway().GetObjects();
                
                using (var streamWriter = new StreamWriter(new FileStream(fullFileName, FileMode.OpenOrCreate)))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        using (var dataReader = new SqlTableReader(source.ToString(), 0, project.MaximumRowsInTable.Value, exportDescription.Parameters))
                        {
                            while (true)
                            {
                                var headers = dataReader.FieldHeaders;
                                if (headers.Any())
                                {
                                    foreach (var field in headers)
                                    {
                                        csvWriter.WriteField(field);
                                    }

                                    csvWriter.NextRecord();
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

                Log.Info("{0} records of '{1}' table successfully exported to {2}.", records, source.Table, fullFileName);
            }
            catch (Exception ex)
            {
                Log.Error("{0} export failed because of exception: {1}", source.Table, ex.Message);
            }
        }

        private static async Task<bool> ReadAll(SqlTableReader dataReader, CsvWriter csvWriter)
        {
            //if (await ReadAll(dataReader, csvWriter))
            //{
            //    return;
            //}

            int records = 0;
            var results = dataReader.ReadAll();
            if (dataReader.ValidationContext.HasErrors)
            {
                Log.Error(dataReader.ValidationContext.ToString());

                return true;
            }

            foreach (var result in results)
            {
                if (result.HasHeaders())
                {
                    result.Headers.ForEach(csvWriter.WriteField);
                    csvWriter.NextRecord();
                }

                foreach (var record in result)
                {
                    foreach (var header in result.Headers)
                    {
                        var value = record[header];
                        if (value is string strValue)
                        {
                            value = strValue.Trim(); // Remove all white spaces
                        }

                        csvWriter.WriteField(value);
                    }

                    records++;

                    await csvWriter.NextRecordAsync();
                }
            }

            return false;
        }


        private static async Task<List<Table>> GetAvailableTablesAsync(SqlConnection sqlConnection, string outputFolder)
        {
            var result = new List<Table>();

            using (var schemaCommand = sqlConnection.CreateGetAvailableTablesSqlCommand())
            {
                using (var schemaReader = await schemaCommand.ExecuteReaderAsync())
                {
                    while (await schemaReader.ReadAsync())
                    {
                        var name = schemaReader.GetString(0);
                        var table = new Table
                        {
                            Name = name,
                            Csv = ExtractTableName(name) + ".csv",
                            Output = outputFolder
                        };

                        result.Add(table);
                    }
                }
            }

            return result;
        }

        private static string ExtractTableName(string tableName)
        {
            var ndx = tableName.LastIndexOf('.');
            return tableName.Substring(ndx + 1).Replace("[", string.Empty).Replace("]", string.Empty);
        }
        #endregion
    }
}
