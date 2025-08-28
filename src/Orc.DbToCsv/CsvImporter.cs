namespace Orc.DbToCsv
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel.Logging;
    using CsvHelper;
    using CsvHelper.Configuration;
    using DataAccess.Database;
    using Microsoft.Data.SqlClient;

    public static class CsvImporter
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static async Task ProcessProjectAsync(string projectFilePath)
        {
            var project = await Project.LoadAsync(projectFilePath);
            if (project is not null)
            {
                await ProcessProjectAsync(project);
            }
        }

        public static async Task ProcessProjectAsync(Project project)
        {
            Log.Info("CSV import project processing started ...");

            try
            {
                var importDescriptions = project.GetCsvToDbImportDescriptions();

                Log.Info("{0} CSV files to import", importDescriptions.Count.ToString());

                foreach (var importDescription in importDescriptions)
                {
                    await ProcessCsvAsync(importDescription);
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

        private static async Task ProcessCsvAsync(CsvToDbImportDescription importDescription)
        {
            var csvFilePath = importDescription.CsvFilePath;
            if (string.IsNullOrWhiteSpace(csvFilePath))
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot process empty csv file path");
            }

            if (!File.Exists(csvFilePath))
            {
                Log.Warning($"CSV file '{csvFilePath}' does not exist, skipping import.");
                return;
            }

            var target = importDescription.Target;
            if (target is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot process null target");
            }

            var factory = DbProviderFactories.GetFactory(target.ProviderName);
            if (factory is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Cannot find provider factory for '{target.ProviderName}'");
            }

            using var connection = factory.CreateConnection();
            if (connection is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot create database connection");
            }

            connection.ConnectionString = target.ConnectionString;

            try
            {
                await connection.OpenAsync();
                Log.Info($"Connected to database for table '{target.Schema}.{target.Table}'");

                // Truncate table if requested
                if (importDescription.TruncateTable)
                {
                    await TruncateTableAsync(connection, target);
                }

                // Import CSV data
                await ImportCsvDataAsync(connection, csvFilePath, target);

                Log.Info($"Successfully imported CSV '{csvFilePath}' to table '{target.Schema}.{target.Table}'");
            }
            catch (Exception ex)
            {
                Log.Error($"Import failed for '{csvFilePath}' to '{target.Table}': {ex.Message}");
                throw;
            }
        }

        private static async Task TruncateTableAsync(DbConnection connection, DatabaseSource target)
        {
            var tableName = string.IsNullOrWhiteSpace(target.Schema) 
                ? $"[{target.Table}]" 
                : $"[{target.Schema}].[{target.Table}]";

            var truncateCommand = $"TRUNCATE TABLE {tableName}";

            Log.Info($"Truncating table {tableName}");

            using var command = connection.CreateCommand();
            command.CommandText = truncateCommand;
            await command.ExecuteNonQueryAsync();

            Log.Info($"Table {tableName} truncated successfully");
        }

        private static void ImportCsvDataAsync(DbConnection connection, string csvFilePath, DatabaseSource target)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null
            };

            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, config);

            // Read the header to get column names
            await csv.ReadAsync();
            csv.ReadHeader();
            var headers = csv.HeaderRecord;

            if (headers is null || headers.Length == 0)
            {
                Log.Warning($"No headers found in CSV file '{csvFilePath}'");
                return;
            }

            Log.Info($"Found {headers.Length} columns in CSV: {string.Join(", ", headers)}");

            var tableName = string.IsNullOrWhiteSpace(target.Schema)
                ? $"[{target.Table}]"
                : $"[{target.Schema}].[{target.Table}]";

            // Build parameterized insert statement
            var columnNames = string.Join(", ", headers.Select(h => $"[{h}]"));
            var parameterNames = string.Join(", ", headers.Select((h, i) => $"@param{i}"));
            var insertSql = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames})";

            Log.Info($"Using SQL: {insertSql}");

            using var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = insertSql;

            // Create parameters
            var parameters = new List<DbParameter>();
            for (int i = 0; i < headers.Length; i++)
            {
                var parameter = insertCommand.CreateParameter();
                parameter.ParameterName = $"@param{i}";
                insertCommand.Parameters.Add(parameter);
                parameters.Add(parameter);
            }

            var recordCount = 0;
            var batchSize = 1000;
            var currentBatch = 0;

            // Use transaction for better performance
            using var transaction = connection.BeginTransaction();
            insertCommand.Transaction = transaction;

            try
            {
                while (await csv.ReadAsync())
                {
                    // Set parameter values from CSV record
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var value = csv.GetField(i);
                        parameters[i].Value = string.IsNullOrWhiteSpace(value) ? DBNull.Value : value;
                    }

                    await insertCommand.ExecuteNonQueryAsync();
                    recordCount++;
                    currentBatch++;

                    if (currentBatch >= batchSize)
                    {
                        Log.Info($"Imported {recordCount} records so far...");
                        currentBatch = 0;
                    }
                }

                await transaction.CommitAsync();
                Log.Info($"Successfully imported {recordCount} records from '{csvFilePath}' to table '{tableName}'");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error($"Transaction rolled back due to error: {ex.Message}");
                throw;
            }
        }
    }
}
