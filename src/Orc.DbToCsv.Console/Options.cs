namespace DbToCsv
{
    using CommandLine;

    public class Options
    {
        [Option('p', "project", Required = true, HelpText = "Path to the xml file defining import project")]
        public string Project { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output folder path")]
        public string OutputFolder { get; set; }
    }
}
