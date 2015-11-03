// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Project.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Markup;
    using System.Xaml;
    using Catel.Logging;

    [ContentProperty("Properties")]
    public class Project
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public Project()
        {
            Tables = new List<Table>();
            Properties = new List<ProjectProperty>();
        }

        private ConnectionString _connectionString;
        public ConnectionString ConnectionString
        {
            get { return _connectionString ?? (_connectionString = Properties.FindTypeOrCreateNew(() => new ConnectionString())); }
        }

        private MaximumRowsInTable _maximumRowsInTable;
        public MaximumRowsInTable MaximumRowsInTable
        {
            get { return _maximumRowsInTable ?? (_maximumRowsInTable = Properties.FindTypeOrCreateNew(() => new MaximumRowsInTable())); }
        }

        private OutputFolder _outputFolder;
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

        public static Project Parse(string xaml)
        {
            var result = (Project)XamlServices.Parse(xaml);
            return result;
        }

        public static Project Load(string path = "project.iprj")
        {
            try
            {
                var xaml = File.ReadAllText(path);
                var result = Parse(xaml);

                result.Validate();
                Log.Info("Loaded project from '{0}'", Path.GetFullPath(path));
                Log.Info("Connection string: '{0}'", result.ConnectionString.Value);
                Log.Info("Maximum rows in table: '{0}'", result.MaximumRowsInTable.Value);
                Log.Info("Tables to convert '{0}':", result.Tables.Count);
                Log.IndentLevel += 2;

                if (string.IsNullOrEmpty(result.OutputFolder.Value))
                {
                    result.OutputFolder.Value = Directory.GetParent(path).FullName;
                }

                foreach (var table in result.Tables)
                {
                    if (string.IsNullOrEmpty(table.Csv))
                    {
                        table.Csv = ExtractTableName(table.Name) + ".csv";
                    }

                    if (table.Output == null)
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
                Log.Info("Please click the run button to start the conversion.");
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
            int ndx = tableName.LastIndexOf('.');
            return tableName.Substring(ndx + 1).Replace("[", string.Empty).Replace("]", string.Empty);
        }

    }
}