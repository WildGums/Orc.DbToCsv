// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Project.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
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
        #region Constants
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        private ConnectionString _connectionString;

        private MaximumRowsInTable _maximumRowsInTable;

        private OutputFolder _outputFolder;
        #endregion

        #region Constructors
        public Project()
        {
            Tables = new List<Table>();
            Properties = new List<ProjectProperty>();
        }
        #endregion

        #region Properties
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
        #endregion

        #region Methods
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
            var result = (Project) XamlServices.Parse(xaml);
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
        #endregion
    }
}