[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.6", FrameworkDisplayName=".NET Framework 4.6")]
[assembly: System.Windows.Markup.XmlnsDefinitionAttribute("http://wildgums/2015", "Orc.DbToCsv")]
[assembly: System.Windows.Markup.XmlnsPrefixAttribute("http://wildgums/2015", "orc")]
public class static ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.DbToCsv
{
    public class ConnectionString : Orc.DbToCsv.StringProjectProperty
    {
        public ConnectionString() { }
    }
    public class DbToCsvExportDescription
    {
        public DbToCsvExportDescription() { }
        public string CsvFilePath { get; set; }
        public Orc.DataAccess.DataSourceParameters Parameters { get; set; }
        public Orc.DataAccess.Database.DatabaseSource Source { get; set; }
    }
    public class static Importer
    {
        public static System.Threading.Tasks.Task ProcessProjectAsync(string projectFilePath, string outputFolderPath) { }
        public static System.Threading.Tasks.Task ProcessProjectAsync(Orc.DbToCsv.Project project) { }
    }
    [System.Windows.Markup.ContentPropertyAttribute("Value")]
    public class Int32ProjectProperty : Orc.DbToCsv.ProjectProperty
    {
        public Int32ProjectProperty() { }
        public int Value { get; set; }
    }
    public class MaximumRowsInTable : Orc.DbToCsv.Int32ProjectProperty
    {
        public MaximumRowsInTable() { }
    }
    public class OutputFolder : Orc.DbToCsv.StringProjectProperty
    {
        public OutputFolder() { }
    }
    public class Parameter
    {
        public Parameter() { }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    [System.Windows.Markup.ContentPropertyAttribute("Properties")]
    public class Project
    {
        public Project() { }
        public Orc.DbToCsv.ConnectionString ConnectionString { get; }
        public Orc.DbToCsv.MaximumRowsInTable MaximumRowsInTable { get; }
        public Orc.DbToCsv.OutputFolder OutputFolder { get; }
        public System.Collections.Generic.List<Orc.DbToCsv.ProjectProperty> Properties { get; set; }
        public Orc.DbToCsv.Provider Provider { get; }
        public Orc.DbToCsv.Schema Schema { get; }
        public System.Collections.Generic.List<Orc.DbToCsv.Table> Tables { get; set; }
        public static System.Threading.Tasks.Task<Orc.DbToCsv.Project> LoadAsync(string path = "project.iprj") { }
        public static Orc.DbToCsv.Project Parse(string xaml) { }
        public void Validate() { }
    }
    public class static ProjectExtensions
    {
        public static System.Threading.Tasks.Task ExportAsync(this Orc.DbToCsv.Project project) { }
        public static System.Collections.Generic.IList<Orc.DbToCsv.DbToCsvExportDescription> GetDbToCsvExportDescriptions(this Orc.DbToCsv.Project project) { }
    }
    public class ProjectProperty
    {
        public ProjectProperty() { }
    }
    public class Provider : Orc.DbToCsv.StringProjectProperty
    {
        public Provider() { }
    }
    public class Schema : Orc.DbToCsv.StringProjectProperty
    {
        public Schema() { }
    }
    [System.Windows.Markup.ContentPropertyAttribute("Value")]
    public class StringProjectProperty : Orc.DbToCsv.ProjectProperty
    {
        public StringProjectProperty() { }
        public string Value { get; set; }
    }
    public class Table
    {
        public Table() { }
        public string ConnectionString { get; set; }
        public string Csv { get; set; }
        public string Name { get; set; }
        public string Output { get; set; }
        public System.Collections.Generic.List<Orc.DbToCsv.Parameter> Parameters { get; set; }
        public string Provider { get; set; }
        public string Schema { get; set; }
        public string TableType { get; set; }
    }
}