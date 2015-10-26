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
            Tables = new List<Table>(4);
            Properties = new List<ProjectProperty>(4);
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
                Log.Info("Connection string: '{0}'", result.ConnectionString);
                Log.Info("Maximum rows in table: '{0}'", result.MaximumRowsInTable);
                Log.Info("Tables to convert '{0}':", result.Tables.Count);
                Log.IndentLevel += 2;
                if (string.IsNullOrEmpty(result.OutputFolder.Value))
                {
                    result.OutputFolder.Value = Directory.GetParent(path).FullName;
                }
                foreach (var table in result.Tables)
                {
                    var folder = table.Output;
                    if (string.IsNullOrEmpty(folder))
                    {
                        folder = result.OutputFolder.Value;
                    }
                    Log.Info("'{0}' to '{1}'", table.Name, Path.Combine(folder, table.Csv));
                }
                Log.IndentLevel -= 2;
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

    }
}