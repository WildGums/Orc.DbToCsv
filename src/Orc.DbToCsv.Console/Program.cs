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
    using DatabaseManagement;

    internal class Program
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Methods
        private static void Main(string[] args)
        {
            InitializeLogManager();
            var commandLine = Environment.CommandLine.GetCommandLine(true);
            var options = new Options();

            options.OutputFolder = "F:\\output";
            options.Project = "F:\\Sample1.iprj";
            
            var serviceLocator = ServiceLocator.Default;
            var commandLineParser = serviceLocator.ResolveType<ICommandLineParser>();
    //        var validationContext = commandLineParser.Parse(commandLine, options);
         //   if (validationContext.HasErrors)
      //      {
     //           Console.WriteLine(validationContext.GetErrors().First().Message);
   //             Environment.Exit(1);
    //        }

            var databaseSource = new DatabaseSource(@"TableType=View,ConnectionString=Data Source=DESKTOP-O27UDHP\\SQLEXPRESS;Initial Catalog=RanttSaaS;Integrated Security=True, Table=Table_To_Test");

            if (options.IsHelp)
            {
                var helpWriterService = serviceLocator.ResolveType<IHelpWriterService>();
                foreach (var helpContent in helpWriterService.GetHelp(options))
                {
                    Console.WriteLine(helpContent);
                }

                return;
            }

            Project project;

            if (!string.IsNullOrEmpty(options.Project))
            {
                project = Project.LoadAsync(options.Project).GetAwaiter().GetResult();
            }
            else
            {
                project = TryGetProjectAutomaticallyAsync().GetAwaiter().GetResult();
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

            Importer.ProcessProjectAsync(project).GetAwaiter().GetResult();
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
