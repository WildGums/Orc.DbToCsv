﻿namespace Orc.DbToCsv
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Markup;
    using System.Xaml;
    using Catel.Logging;
    using DataAccess;

    [ContentProperty(nameof(Properties))]
    public class Project
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private ConnectionString? _connectionString;
        private Schema? _schema;
        private MaximumRowsInTable? _maximumRowsInTable;
        private OutputFolder? _outputFolder;
        private Provider? _provider;

        public Project()
        {
            Tables = new List<Table>();
            Properties = new List<ProjectProperty>();
        }

        public Provider Provider
        {
            get { return _provider ?? (_provider = Properties.FindTypeOrCreateNew(() => new Provider())); }
        }

        public Schema Schema
        {
            get { return _schema ?? (_schema = Properties.FindTypeOrCreateNew(() => new Schema())); }
        }

        public ConnectionString ConnectionString
        {
            get { return _connectionString ?? (_connectionString = Properties.FindTypeOrCreateNew(() => new ConnectionString())); }
        }

        public MaximumRowsInTable MaximumRowsInTable
        {
            get { return _maximumRowsInTable ?? (_maximumRowsInTable = Properties.FindTypeOrCreateNew(() => new MaximumRowsInTable())); }
        }

        public OutputFolder OutputFolder
        {
            get { return _outputFolder ?? (_outputFolder = Properties.FindTypeOrCreateNew(() => new OutputFolder())); }
        }

        public List<Table> Tables { get; set; }
        public List<ProjectProperty> Properties { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(ConnectionString.Value))
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Connection string cannot be empty");
            }

            foreach (var table in Tables)
            {
                if (string.IsNullOrEmpty(table.Name))
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Table name cannot be empty");
                }
            }
        }

        public static Project? Parse(string xaml)
        {
            var result = (Project)XamlServices.Parse(xaml);
            return result;
        }

        public static async Task<Project?> LoadAsync(string path = "project.iprj")
        {
            try
            {
                var xaml = await File.ReadAllTextAsync(path);
                var result = Parse(xaml);
                if (result is null)
                {
                    return result;
                }

                result.Validate();
                Log.Info("Loaded project from '{0}'", Path.GetFullPath(path));
                Log.Info("Connection string: '{0}'", result.ConnectionString.Value);
                Log.Info("Maximum rows in table: '{0}'", result.MaximumRowsInTable.Value);
                Log.Info("Tables to convert '{0}':", result.Tables.Count);
                Log.IndentLevel += 2;

                if (string.IsNullOrEmpty(result.OutputFolder.Value))
                {
                    result.OutputFolder.Value = Directory.GetParent(path)?.FullName ?? string.Empty;
                }

                foreach (var table in result.Tables)
                {
                    if (string.IsNullOrEmpty(table.Csv))
                    {
                        table.Csv = ExtractTableName(table.Name) + ".csv";
                    }

                    if (table.Output is null)
                    {
                        table.Output = string.Empty;
                    }

                    if (!Path.IsPathRooted(table.Output))
                    {
                        table.Output = Path.Combine(result.OutputFolder.Value, table.Output);
                    }

                    Log.Info("'{0}' to '{1}'", table.Name, Path.Combine(table.Output, table.Csv));
                }

                Log.IndentLevel -= 2;

                Log.Info("");

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        private static string ExtractTableName(string tableName)
        {
            var ndx = tableName.LastIndexOf('.');
            return tableName.Substring(ndx + 1).Replace("[", string.Empty).Replace("]", string.Empty);
        }
    }
}
