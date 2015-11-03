namespace Orc.DbToCsv.TaskRunner
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using Catel.Logging;
    using Models;
    using Orchestra.Models;
    using Orchestra.Services;
    using Views;

    public class TaskRunnerService: ITaskRunnerService
    {
        private static ILog Log = LogManager.GetCurrentClassLogger();

        public AboutInfo GetAboutInfo()
        {
            return new AboutInfo();
        }

        public object GetViewDataContext()
        {
            return new Settings();
        }

        public FrameworkElement GetView()
        {
            return new SettingsView();
        }

        public async Task RunAsync(object dataContext)
        {
            var settings = dataContext as Settings;
            var project = new Project();
            project.ConnectionString.Value = settings.ConnectionString;
            project.MaximumRowsInTable.Value = settings.MaximumRowsInTable;
            project.OutputFolder.Value = settings.OutputDirectory;
            project.Tables = settings.Tables;
            Importer.ProcessProject(project);
        }

        public Size GetInitialWindowSize()
        {
            return Size.Empty;
        }

        public string Title { get { return "Orc.DbToCsv"; } }
        public bool ShowCustomizeShortcutsButton { get { return false; } }
        public event EventHandler TitleChanged;
    }
}
