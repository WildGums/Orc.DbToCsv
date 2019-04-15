// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Collections.Generic;
    using System.IO;
    using Catel;
    using DatabaseManagement;

    public static class ProjectExtensions
    {
        #region Methods
        public static IList<DbToCsvExportDescription> GetDbToCsvExportDescriptions(this Project project)
        {
            Argument.IsNotNull(() => project);

            var dbToCsvExportDescriptions = new List<DbToCsvExportDescription>();
            foreach (var table in project.Tables)
            {
                var databaseSource = new DatabaseSource
                {
                    Table = table.Name,
                    ConnectionString = string.IsNullOrWhiteSpace(table.ConnectionString) ? project.ConnectionString.Value : table.ConnectionString,
                    ProviderName = string.IsNullOrWhiteSpace(table.Provider) ? project.Provider.Value : table.Provider
                };

                databaseSource.SetProperty(nameof(DatabaseSource.TableType), table.TableType);

                var outputFolderPath = string.IsNullOrEmpty(table.Output) ? project.OutputFolder.Value : table.Output;
                var destinationFile = Path.Combine(outputFolderPath, table.Csv);

                dbToCsvExportDescriptions.Add(new DbToCsvExportDescription
                {
                    CsvFilePath = destinationFile,
                    Source = databaseSource
                });
            }

            return dbToCsvExportDescriptions;
        }
        #endregion
    }
}
