namespace Orc.DbToCsv
{
    using CommandLine;

    public class Options : ContextBase
    {
        [Option("p", "project", IsMandatory = false, HelpText = "Path to the xml file defining import project")]
        public string? Project { get; set; }

        [Option("o", "output", IsMandatory = false, HelpText = "Output folder path")]
        public string? OutputFolder { get; set; }

        [Option("i", "import", IsMandatory = false, HelpText = "Import mode: import CSV files to database instead of exporting")]
        public bool ImportMode { get; set; } = false;

        [Option("t", "truncate", IsMandatory = false, HelpText = "Truncate tables before importing (only applies in import mode)")]
        public bool TruncateTables { get; set; } = false;
    }
}
