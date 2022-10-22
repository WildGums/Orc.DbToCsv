namespace Orc.DbToCsv
{
    using System.Collections.Generic;

    public class Table
    {
        public Table()
        {
            Schema = string.Empty;
            TableType = string.Empty;
            Name = string.Empty;
            Csv = string.Empty;
            Output = string.Empty;
            ConnectionString = string.Empty;
            Parameters = new List<Parameter>();
        }

        public string Schema { get; set; }
        public string TableType { get; set; }
        public string Name { get; set; }
        public string Csv { get; set; }
        public string Output { get; set; }
        public string ConnectionString { get; set; }
        public string Provider { get; set; } = "System.Data.SqlClient";
        public List<Parameter> Parameters { get; set; }
    }
}
