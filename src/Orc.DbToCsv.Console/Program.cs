namespace Orc.DbToCsv
{
    using System;
    using System.Data.Common;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.IoC;
    using Catel.Logging;
    using CommandLine;
    using DataAccess.Database;
    using Microsoft.Data.SqlClient;

    internal class Program
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            InitializeLogManager();

            var sqLiteProviderInfo = new DbProviderInfo("SQLite Data Provider",
                "System.Data.SQLite",
                ".NET Framework Data Provider for SQLite",
                "System.Data.SQLite.SQLiteFactory, System.Data.SQLite, Version=1.0.110.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139");

            DbProvider.RegisterProvider(sqLiteProviderInfo);

            var oracleProviderInfo = new DbProviderInfo("ODP.NET, Managed Driver",
                "Oracle.ManagedDataAccess.Client",
                "Oracle Data Provider for .NET, Managed Driver",
                "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess, Version=4.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342");

            DbProvider.RegisterProvider(oracleProviderInfo);

            DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
            
            var options = new Options();
            
            var serviceLocator = ServiceLocator.Default;
            var commandLineParser = serviceLocator.ResolveRequiredType<ICommandLineParser>();
            var validationContext = commandLineParser.Parse(args, options);
            if (validationContext.HasErrors)
            {
                Console.WriteLine(validationContext.GetErrors().First().Message);
                Environment.Exit(1);
            }

            if (options.IsHelp)
            {
                var helpWriterService = serviceLocator.ResolveRequiredType<IHelpWriterService>();
                foreach (var helpContent in helpWriterService.GetHelp(options))
                {
                    Console.WriteLine(helpContent);
                }

                return;
            }

            var project = !string.IsNullOrEmpty(options.Project) 
                ? Project.LoadAsync(options.Project).GetAwaiter().GetResult()
                : TryGetProjectAutomaticallyAsync().GetAwaiter().GetResult();

            if (project is null)
            {
                Log.Warning("Unable to locate the project to process.");
                Environment.Exit(1);
            }
            else
            {
                options.OutputFolder = project.OutputFolder.Value;
            }

            // Apply truncate option to all tables if specified
            if (options is { TruncateTables: true, ImportMode: true })
            {
                foreach (var table in project.Tables)
                {
                    table.TruncateTable = true;
                }
                Log.Info("Truncate option enabled for all tables");
            }

            // Choose operation based on mode
            if (options.ImportMode)
            {
                Log.Info("Running in import mode: CSV → Database");
                project.ImportAsync().GetAwaiter().GetResult();
            }
            else
            {
                Log.Info("Running in export mode: Database → CSV");
                project.ExportAsync().GetAwaiter().GetResult();
            }
        }

        private static async Task<Project?> TryGetProjectAutomaticallyAsync()
        {
            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            var candidates = directoryInfo.GetFiles("*.iprj");

            foreach (var candidate in candidates)
            {
                try
                {
                    var project = await Project.LoadAsync(candidate.FullName);
                    return project;
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }
        private static void InitializeLogManager()
        {
            LogManager.IgnoreCatelLogging = true;
            LogManager.AddListener(new BriefConsoleLogger());
        }
    }
}
