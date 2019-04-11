// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Importer.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Catel.Logging;
    using CsvHelper;
    using System.Linq;

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
                using (var sqlConnection = new SqlConnection(project.ConnectionString.Value))
                {
                    sqlConnection.Open();

                    var tables = project.Tables;
                    if (project.Tables == null || project.Tables.Count == 0)
                    {
                        tables = await GetAvailableTablesAsync(sqlConnection, project.OutputFolder.Value);
                    }

                    Log.Info("{0} tables to process", tables.Count.ToString());

                    foreach (var table in tables)
                    {
                        await ProcessTableAsync(sqlConnection, table, project);
                    }
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

        private static async Task ProcessTableAsync(DbConnection sqlConnection, Table table, Project project)
        {
            var outputFolderPath = string.IsNullOrEmpty(table.Output) ? project.OutputFolder.Value : table.Output;

            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }
            
            var fullFileName = Path.Combine(outputFolderPath, table.Csv);
            var records = 0;

            try
            {
                if (File.Exists(fullFileName))
                {
                    File.Delete(fullFileName);
                }

                using (var streamWriter = new StreamWriter(new FileStream(fullFileName, FileMode.OpenOrCreate)))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        using (var dataReader = sqlConnection.GetRecordsReader(project, table.Name))
                        {
                            var schemaTable = dataReader.GetSchemaTable();
                            var schemaRows = schemaTable?.Rows;
                            var schemaRowsCount = schemaRows?.Count ?? 0;
                            for (var i = 0; i < schemaRowsCount; i++)
                            {
                                var row = schemaRows[i];
                                var columnName = row["ColumnName"] as string;
                                csvWriter.WriteField(columnName);
                            }

                            csvWriter.NextRecord();

                            while (dataReader.Read())
                            {
                                for (var i = 0; i < schemaRowsCount; i++)
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
                        }
                    }
                }
                
                Log.Info("{0} records of '{1}' table successfully exported to {2}.", records, table.Name, fullFileName);
            }
            catch (Exception ex)
            {
                Log.Error("{0} export failed because of exception: {1}", table.Name, ex.Message);
            }
        }

        private static async Task<List<Tuple<string, string>>> GetTableSchemaAsync(SqlConnection sqlConnection, string tableName)
        {
            var result = new List<Tuple<string, string>>();

            Log.Info("");
            Log.Info("> Processing table '{0}'", tableName);

            using (var schemaCommand = sqlConnection.CreateGetTableSchemaSqlCommand(tableName))
            {
                using (var schemaReader = await schemaCommand.ExecuteReaderAsync())
                {
                    var processedNames = new HashSet<string>();

                    while (schemaReader.Read())
                    {
                        var name = schemaReader.GetString(0);
                        var stringColumnType = schemaReader.GetString(1);

                        if (processedNames.Contains(name))
                        {
                            continue;
                        }

                        processedNames.Add(name);

                        switch (stringColumnType)
                        {
                            case "bit":
                                result.Add(new Tuple<string, string>(name, "boolean"));
                                Log.Info("    Field name '{0}' is a '{1}' type.", name, "boolean");
                                break;

                            case "varchar":
                            case "nvarchar":
                            case "nchar":
                            case "text":
                            case "char":
                                result.Add(new Tuple<string, string>(name, "string"));
                                Log.Info("    Field name '{0}' is a '{1}' type.", name, "string");
                                break;

                            case "int":
                            case "smallint":
                            case "tinyint":
                            case "bigint":
                                result.Add(new Tuple<string, string>(name, "int"));
                                Log.Info("    Field name '{0}' is a '{1}' type.", name, "int");
                                break;

                            case "float":
                            case "real":
                            case "numeric":
                            case "money":
                            case "smallmoney":
                            case "decimal":
                                result.Add(new Tuple<string, string>(name, "float"));
                                Log.Info("    Field name '{0}' is a '{1}' type.", name, "float");
                                break;

                            case "datetime":
                            case "smalldatetime":
                            case "datetime2":
                            case "date":
                                result.Add(new Tuple<string, string>(name, "datetime"));
                                Log.Info("    Field name '{0}' is a '{1}' type.", name, "datetime");
                                break;

                            case "time":
                                result.Add(new Tuple<string, string>(name, "time"));
                                Log.Info("    Field name '{0}' is a '{1}' type.", name, "time");
                                break;

                            case "timestamp":
                                // Ignore for now
                                break;

                            default:
                                result.Add(new Tuple<string, string>(name, "string"));
                                Log.Info("    Field name '{0}' did not have a match for type '{1}', setting it to 'string'.", name, stringColumnType);
                                break;
                        }
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

        private static async Task<List<Table>> GetAvailableTablesAsync(SqlConnection sqlConnection, string outputFolder)
        {
            var result = new List<Table>();

            using (var schemaCommand = sqlConnection.CreateGetAvailableTablesSqlCommand())
            {
                using (var schemaReader = await schemaCommand.ExecuteReaderAsync())
                {
                    while (await schemaReader.ReadAsync())
                    {
                        var table = new Table();
                        table.Name = schemaReader.GetString(0);
                        table.Csv = ExtractTableName(table.Name) + ".csv";
                        table.Output = outputFolder;
                        result.Add(table);
                    }
                }
            }

            return result;
        }
        #endregion
    }
}
