// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Options.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using CommandLine;

    public class Options
    {
        #region Properties
        [Option('p', "project", Required = false, HelpText = "Path to the xml file defining import project")]
        public string Project { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output folder path")]
        public string OutputFolder { get; set; }
        #endregion
    }
}