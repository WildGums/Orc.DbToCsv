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
    using System.Xaml;
    using Catel.Logging;

    public class Project
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public Project()
        {
            Tables = new List<Table>(4);
        }

        public string ConnectionString { get; set; }
        public int MaximumRowsInTable { get; set; }
        public string OutputFolder { get; set; }
        public List<Table> Tables { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(ConnectionString))
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
                if (string.IsNullOrEmpty(result.OutputFolder))
                {
                    result.OutputFolder = Directory.GetParent(path).FullName;
                }
                foreach (var table in result.Tables)
                {
                    var folder = table.Output;
                    if (string.IsNullOrEmpty(folder))
                    {
                        folder = result.OutputFolder;
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