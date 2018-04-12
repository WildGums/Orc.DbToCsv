// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BriefConsoleLogger.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System;
    using Catel.Logging;

    public class BriefConsoleLogger : LogListenerBase
    {
        #region Methods
        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            if (string.Equals(log.Tag, "Orc.CommandLine.CommandLineParser"))
            {
                return;
            }

            if (logEvent == LogEvent.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (logEvent == LogEvent.Warning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine("{0}", message);
        }
        #endregion
    }
}