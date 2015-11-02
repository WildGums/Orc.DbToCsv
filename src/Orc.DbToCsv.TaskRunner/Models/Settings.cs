namespace Orc.DbToCsv.TaskRunner.Models
{
    using Catel.Data;

    public class Settings : ModelBase
    {
        public Settings()
        {
            ConnectionString = @"Data Source=localhost;Initial Catalog=RanttSaaS;Integrated Security=True";
            MaximumRowsInTable = 500;
            OutputDirectory = "./Output";
        }
        public string ConnectionString { get; set; }

        public int MaximumRowsInTable { get; set; }

        public string OutputDirectory { get; set; }
    }
}