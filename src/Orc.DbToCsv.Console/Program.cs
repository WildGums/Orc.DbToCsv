// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.IoC;
    using Catel.Logging;
    using CommandLine;
    using DataAccess.Database;

    internal class Program
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Methods
        private static void Main(string[] args)
        {
            InitializeLogManager();

            var sqLiteProviderInfo = new DbProviderInfo
            {
                Name = "SQLite Data Provider",
                InvariantName = "System.Data.SQLite",
                Description = ".NET Framework Data Provider for SQLite",
                AssemblyQualifiedName = "System.Data.SQLite.SQLiteFactory, System.Data.SQLite, Version=1.0.110.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139"
            };
            DbProvider.RegisterProvider(sqLiteProviderInfo);

            var oracleProviderInfo = new DbProviderInfo
            {
                Name = "ODP.NET, Managed Driver",
                InvariantName = "Oracle.ManagedDataAccess.Client",
                Description = "Oracle Data Provider for .NET, Managed Driver",
                AssemblyQualifiedName = "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess, Version=4.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342"
            };
            DbProvider.RegisterProvider(oracleProviderInfo);

            var commandLine = Environment.CommandLine.GetCommandLine(true);
            var options = new Options();
            
            var serviceLocator = ServiceLocator.Default;
            var commandLineParser = serviceLocator.ResolveType<ICommandLineParser>();
            var validationContext = commandLineParser.Parse(commandLine, options);
            if (validationContext.HasErrors)
            {
                Console.WriteLine(validationContext.GetErrors().First().Message);
                Environment.Exit(1);
            }

            if (options.IsHelp)
            {
                var helpWriterService = serviceLocator.ResolveType<IHelpWriterService>();
                foreach (var helpContent in helpWriterService.GetHelp(options))
                {
                    Console.WriteLine(helpContent);
                }

                return;
            }

            var project = !string.IsNullOrEmpty(options.Project) 
                ? Project.LoadAsync(options.Project).GetAwaiter().GetResult()
                : TryGetProjectAutomaticallyAsync().GetAwaiter().GetResult();

            if (project == null)
            {
                Log.Warning("Unable to locate the project to process.");
                Environment.Exit(1);
            }
            else
            {
                options.OutputFolder = project.OutputFolder.Value;
            }

            project.ExportAsync().GetAwaiter().GetResult();
        }

        private static async Task<Project> TryGetProjectAutomaticallyAsync()
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
                    continue;
                }
            }

            return null;
        }
        private static void InitializeLogManager()
        {
            LogManager.IgnoreCatelLogging = true;
            LogManager.AddListener(new BriefConsoleLogger());
        }

        #endregion
    }
}
