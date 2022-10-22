namespace Orc.DbToCsv
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.Database;

    public static class ProjectExtensions
    {
        public static Task ExportAsync(this Project project)
        {
            ArgumentNullException.ThrowIfNull(project);

            return Importer.ProcessProjectAsync(project);
        }

        public static IList<DbToCsvExportDescription> GetDbToCsvExportDescriptions(this Project project)
        {
            ArgumentNullException.ThrowIfNull(project);

            var dbToCsvExportDescriptions = new List<DbToCsvExportDescription>();
            foreach (var table in project.Tables)
            {
                var databaseSource = new DatabaseSource
                {
                    Schema = (string.IsNullOrWhiteSpace(table.Schema) ? project.Schema.Value : table.Schema) ?? string.Empty,
                    Table = table.Name,
                    ConnectionString = (string.IsNullOrWhiteSpace(table.ConnectionString) ? project.ConnectionString.Value : table.ConnectionString) ?? string.Empty,
                    ProviderName = (string.IsNullOrWhiteSpace(table.Provider) ? project.Provider.Value : table.Provider) ?? string.Empty
                };

                databaseSource.SetProperty(nameof(DatabaseSource.TableType), table.TableType);

                var outputFolderPath = (string.IsNullOrEmpty(table.Output) ? project.OutputFolder.Value : table.Output) ?? string.Empty;
                var destinationFile = Path.Combine(outputFolderPath, table.Csv);

                dbToCsvExportDescriptions.Add(new DbToCsvExportDescription
                {
                    CsvFilePath = destinationFile,
                    Source = databaseSource,
                    Parameters = new DataSourceParameters
                    {
                        Parameters = table.Parameters.Select(x => new DataSourceParameter
                        {
                            Name = x.Name ?? throw new InvalidOperationException("Cannot handle null parameter"),
                            Value = x.Value
                        }).ToList()
                    },
                });
            }

            return dbToCsvExportDescriptions;
        }
    }
}
