// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Importer.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Logging;
    using CsvHelper;
    using DatabaseManagement;
    using Npgsql;
    using SqlKata;
    using SqlKata.Compilers;

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
                //var p = new PostgreSQLDbProvider();
                var p = DbProvider.GetRegisteredProviders()["Npgsql"];
                var connection1 = p.CreateConnection(new DatabaseSource(@"ConnectionString=Server=127.0.0.1;Port=5432;Database=newDb;User Id=postgres;Password=postleovg2562;, Table=NewPT"));
                await connection1.OpenConnectionAsync();

                var provider1 = connection1.GetDbProvider();
               // var results1 = await connection1.QuerySqlAsync("select * from \"newPT\" limit 3");

               //var provider
                var p2 = DbProvider.GetRegisteredProviders()["System.Data.SqlClient"];
                var connection2 = p2.CreateConnection(new DatabaseSource($@"ConnectionString={project.ConnectionString.Value}"));
                await connection2.OpenConnectionAsync();

                var provider2 = connection2.GetDbProvider();
              //  var results2 = await connection2.QuerySqlAsync($"select * from test_1");
                
               using (var connection = connection1/*p2.CreateConnection(new DatabaseSource($@"ConnectionString={project.ConnectionString.Value}"))*/)
                {
                   // connection.ConnectionString = project.ConnectionString.Value;// "Server=127.0.0.1;Port=5432;Database=newDb;User Id=postgres;Password=postleovg2562;";
                 //  await connection.OpenConnectionAsync1();

                    var tables = project.Tables;
                    if (project.Tables == null || project.Tables.Count == 0)
                    {
                       // tables = await GetAvailableTablesAsync(connection, project.OutputFolder.Value);
                    }

                    Log.Info("{0} tables to process", tables.Count.ToString());

                    foreach (var table in tables)
                    {
                        await ProcessTableAsync(connection, table, project);
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
                            if (dataReader == null)
                            {
                                return;
                            }

                            while (dataReader.HasRows)
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

                                while (await dataReader.ReadAsync())
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

                                await dataReader.NextResultAsync();
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
        #endregion
    }
}
