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
    using Catel.IoC;
    using Catel.Logging;
    using CommandLine;

    internal class Program
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #region Methods
        private static void Main(string[] args)
        {
            InitializeLogManager();
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

            Project project = null;
            if (!string.IsNullOrEmpty(options.Project))
            {
                project = Project.Load(options.Project);
            }
            else
            {
                project = TryGetProjectAutomatically();
            }

            if (project == null)
            {
                Log.Warning("Unable to locate the project to process.");
                Environment.Exit(1);
            }
            else
            {
                options.OutputFolder = project.OutputFolder.Value;
            }

            var outputFolder = string.IsNullOrEmpty(options.OutputFolder) ? Directory.GetCurrentDirectory() : options.OutputFolder;

            Importer.ProcessProject(project);
        }

        private static Project TryGetProjectAutomatically()
        {
            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            var candidates = directoryInfo.GetFiles("*.iprj");

            foreach (FileInfo candidate in candidates)
            {
                try
                {
                    var project = Project.Load(candidate.FullName);
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