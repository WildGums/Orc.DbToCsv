[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.5", FrameworkDisplayName=".NET Framework 4.5")]
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
    public class static ICollectionExtensions
    {
        public static TTarget FindTypeOrCreateNew<T, TTarget>(this System.Collections.Generic.ICollection<T> collection, System.Func<TTarget> func)
        
            where TTarget : T { }
    }
    public class static Importer
    {
        public static void ProcessProject(string projectFilePath, string outputFolderPath) { }
        public static void ProcessProject(Orc.DbToCsv.Project project) { }
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
    [System.Windows.Markup.ContentPropertyAttribute("Properties")]
    public class Project
    {
        public Project() { }
        public Orc.DbToCsv.ConnectionString ConnectionString { get; }
        public Orc.DbToCsv.MaximumRowsInTable MaximumRowsInTable { get; }
        public Orc.DbToCsv.OutputFolder OutputFolder { get; }
        public System.Collections.Generic.List<Orc.DbToCsv.ProjectProperty> Properties { get; set; }
        public System.Collections.Generic.List<Orc.DbToCsv.Table> Tables { get; set; }
        public static Orc.DbToCsv.Project Load(string path = "project.iprj") { }
        public static Orc.DbToCsv.Project Parse(string xaml) { }
        public void Validate() { }
    }
    public class ProjectProperty
    {
        public ProjectProperty() { }
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
        public string Csv { get; set; }
        public string Name { get; set; }
        public string Output { get; set; }
    }
}