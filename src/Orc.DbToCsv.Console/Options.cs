namespace Orc.DbToCsv
{
    using CommandLine;

    public class Options : ContextBase
    {
        [Option("p", "project", IsMandatory = false, HelpText = "Path to the xml file defining import project")]
        public string? Project { get; set; }

        [Option("o", "output", IsMandatory = false, HelpText = "Output folder path")]
        public string? OutputFolder { get; set; }
    }
}
