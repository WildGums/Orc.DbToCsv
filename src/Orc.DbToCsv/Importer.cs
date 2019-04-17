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
    using System.Threading.Tasks;
    using Catel.Data;
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
                
                //try
                //{
                //    var gateway = source.CreateGateway();
                //    var reader = gateway.GetRecords();
                //    while (reader.Read())
                //    {
                //        var test = reader.GetValue(0);
                //    }

                //    var count = gateway.GetCount();

                //    var tables = gateway.GetObjects();
                //    var parameters = gateway.GetQueryParameters();
                //}
                //catch(Exception ex)
                //{
                //    Console.Write(ex);
                //}
                
                using (var streamWriter = new StreamWriter(new FileStream(fullFileName, FileMode.OpenOrCreate)))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        var validationContext = new ValidationContext();
                        using (var dataReader = new SqlTableReader(source.ToString(), validationContext, 0, project.MaximumRowsInTable.Value))
                        {
                            while (true)
                            {
                                var currentRecord = dataReader.FieldHeaders;
                                foreach (var field in currentRecord)
                                {
                                    csvWriter.WriteField(field);
                                }
                                csvWriter.NextRecord();

                                while (await dataReader.ReadAsync())
                                {
                                    for (var i = 0; i < currentRecord.Length; i++)
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
