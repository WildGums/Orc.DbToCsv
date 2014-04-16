namespace Orc.DbToCsv.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Orc.DbToCsv.Console.Implementations;
    using Orc.DbToCsv.Core;

    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            CommandLine.Parser.Default.ParseArgumentsStrict(
                args,
                options,
                () =>
                {
                    Console.WriteLine("Arguments are not valid. Use -help flag to start app correctly.");
                    Environment.Exit(1);
                });

            var project = Project.Load(options.Project);

            string outputFolder = string.IsNullOrEmpty(options.OutputFolder) ?
                Directory.GetCurrentDirectory() :
                options.OutputFolder;

            Importer.ProcessProject(project, outputFolder, new ConsoleWriter());
        }
    }
}
