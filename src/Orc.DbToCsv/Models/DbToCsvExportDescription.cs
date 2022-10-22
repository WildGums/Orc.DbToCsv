namespace Orc.DbToCsv
{
    using DataAccess;
    using DataAccess.Database;

    public class DbToCsvExportDescription
    {
        public DatabaseSource? Source { get; set; }
        public DataSourceParameters? Parameters { get; set; }
        public string? CsvFilePath { get; set; }
    }
}
