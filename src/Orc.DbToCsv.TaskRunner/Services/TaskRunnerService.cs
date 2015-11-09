namespace Orc.DbToCsv.TaskRunner
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using Catel.Configuration;
    using Catel.Logging;
    using Models;
    using Orchestra.Models;
    using Orchestra.Services;
    using Views;

    public class TaskRunnerService: ITaskRunnerService
    {
        private readonly IConfigurationService _configurationService;
        private static ILog Log = LogManager.GetCurrentClassLogger();

        public TaskRunnerService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public AboutInfo GetAboutInfo()
        {
            return new AboutInfo();
        }

        public object GetViewDataContext()
        {
            var settings = new Settings();
            var lastProjectPath = _configurationService.GetValue("LastProjectPath", string.Empty);
            if (!string.IsNullOrEmpty(lastProjectPath) && File.Exists(lastProjectPath))
            {
                settings.ProjectFile = lastProjectPath;
            }
            return settings;
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
            if (File.Exists(settings.ProjectFile))
            {
                _configurationService.SetValue("LastProjectPath", settings.ProjectFile);
            }
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
