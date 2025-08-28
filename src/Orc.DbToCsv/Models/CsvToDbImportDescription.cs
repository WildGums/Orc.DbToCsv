namespace Orc.DbToCsv
{
    using DataAccess;
    using DataAccess.Database;

    public class CsvToDbImportDescription
    {
        public string? CsvFilePath { get; set; }
        public DatabaseSource? Target { get; set; }
        public DataSourceParameters? Parameters { get; set; }
        public bool TruncateTable { get; set; } = false;
    }
}
