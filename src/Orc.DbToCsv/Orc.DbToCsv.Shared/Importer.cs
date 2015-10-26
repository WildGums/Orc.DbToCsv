// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Importer.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Catel.Logging;
    using CsvHelper;

    public static class Importer
    {
        private static ILog Log = LogManager.GetCurrentClassLogger();
        #region Methods

        public static void ProcessProject(string projectFilePath, string outputFolderPath)
        {
            var project = Project.Load(projectFilePath);

            ProcessProject(project, outputFolderPath);
        }

        public static void ProcessProject(Project project, string outputFolderPath)
        {
            Log.Info("Project processing started ...");

            using (var sqlConnection = new SqlConnection(project.ConnectionString))
            {
                sqlConnection.Open();
                var tables = project.Tables;
                if (project.Tables == null || project.Tables.Count == 0)
                {
                    tables = GetAvailableTables(sqlConnection, project.OutputFolder);
                }

                Log.Info("{0} tables to process", tables.Count.ToString());
                foreach (var table in tables)
                {
                    ProcessTable(sqlConnection, table, project);
                }
            }
        }

        private static void ProcessTable(SqlConnection sqlConnection, Table table, Project project)
        {
            var outputFolderPath = string.IsNullOrEmpty(table.Output) ? project.OutputFolder : table.Output;

            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            string fullFileName = Path.Combine(outputFolderPath, table.Csv);
            int records = 0;

            try
            {
                List<Tuple<string, string>> schema = GetTableSchema(sqlConnection, table.Name);
                if (schema.Count == 0)
                {
                    Log.Warning("No columns was found in the '{0}' table to export into a csv file.", table.Name);
                    return;
                }

                if (File.Exists(fullFileName))
                {
                    File.Delete(fullFileName);
                }

                using (var streamWriter = new StreamWriter(new FileStream(fullFileName, FileMode.OpenOrCreate)))
                {
                    var csvWriter = new CsvWriter(streamWriter);
                    // Write Header
                    foreach (var tuple in schema)
                    {
                        csvWriter.WriteField(tuple.Item1);
                    }
                    csvWriter.NextRecord();

                    // Write records
                    var query = ConstructRecordQuery(table.Name, schema, project.MaximumRowsInTable);
                    using (var command = new SqlCommand(query) {Connection = sqlConnection})
                    {
                        using (var dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                for (int i = 0; i < schema.Count; i++)
                                {
                                    object value = dataReader.GetValue(i);
                                    csvWriter.WriteField(value);
                                }

                                records++;
                                csvWriter.NextRecord();
                            }
                        }
                    }
                }

                Log.Info("{0} records of '{1}' table succesfully exported to {2}.", records, table.Name, table.Csv);
            }
            catch (Exception ex)
            {
                Log.Error("{0} export failed because of exception: {1}", table.Name, ex.Message);
            }
        }

        private static string ConstructRecordQuery(string tableName, List<Tuple<string, string>> schema, int maximumRowsInTable)
        {
            string columns = string.Join("], [", schema.Select(t => t.Item1));
            string top = maximumRowsInTable > 0 ? string.Format("TOP {0}", maximumRowsInTable) : string.Empty;

            return string.Format("SELECT {0} [{1}] FROM {2}", top, columns, tableName);
        }

        private static List<Tuple<string, string>> GetTableSchema(SqlConnection sqlConnection, string tableName)
        {
            List<Tuple<string, string>> result = new List<Tuple<string, string>>();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("SELECT c.name AS Name, t.name AS columnType");
            stringBuilder.AppendLine("FROM sys.columns c INNER JOIN sys.types t ON c.system_type_id = t.system_type_id");
            stringBuilder.AppendFormat(
                " WHERE c.object_id = OBJECT_ID('{0}') and t.name<>'sysname' ",
                tableName);

            Log.Info("");
            Log.Info("> Processing table '{0}'", tableName);

            string commandText = stringBuilder.ToString();
            using (SqlCommand schemaCommand = new SqlCommand(commandText) {Connection = sqlConnection})
            {
                using (SqlDataReader schemaReader = schemaCommand.ExecuteReader())
                {

                    var processedNames = new HashSet<string>();

                    while (schemaReader.Read())
                    {
                        string name = schemaReader.GetString(0);
                        string stringColumnType = schemaReader.GetString(1);

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
                                Log.Info("    Field name '{0}' is a '{1}' type.", name, "sting");
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
                            case "timestamp":
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
            int ndx = tableName.LastIndexOf('.');
            return tableName.Substring(ndx + 1).Replace("[", string.Empty).Replace("]", string.Empty);
        }

        private static List<Table> GetAvailableTables(SqlConnection sqlConnection, string outputFolder)
        {
            const string CommandText = "SELECT '['+TABLE_SCHEMA+'].['+ TABLE_NAME + ']' FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_SCHEMA, TABLE_NAME";

            var result = new List<Table>();

            using (var schemaCommand = new SqlCommand(CommandText) {Connection = sqlConnection, CommandTimeout = 300})
            {
                using (var schemaReader = schemaCommand.ExecuteReader())
                {
                    while (schemaReader.Read())
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