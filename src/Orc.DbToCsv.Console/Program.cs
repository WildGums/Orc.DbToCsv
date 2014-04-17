namespace DbToCsv
{
    using System;
    using System.IO;

    using DbToCsv.Implementations;

    using Orc.DbToCsv;

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
                Console.WriteLine("Unable to locate the project to process.");
                Environment.Exit(1);
            }
            

            string outputFolder = string.IsNullOrEmpty(options.OutputFolder) ?
                Directory.GetCurrentDirectory() :
                options.OutputFolder;

            Importer.ProcessProject(project, outputFolder, new ConsoleWriter());
        }

        private static Project TryGetProjectAutomatically()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            FileInfo[] candidates = directoryInfo.GetFiles("*.iprj");
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
    }
}
