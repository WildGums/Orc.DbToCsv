// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Options.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using CommandLine;

    public class Options : ContextBase
    {
        #region Properties
        [Option("p", "project", IsMandatory = false, HelpText = "Path to the xml file defining import project")]
        public string Project { get; set; }

        [Option("o", "output", IsMandatory = false, HelpText = "Output folder path")]
        public string OutputFolder { get; set; }
        #endregion
    }
}
