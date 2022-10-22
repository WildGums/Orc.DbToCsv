namespace Orc.DbToCsv
{
    using System;
    using Catel.Logging;

    public class BriefConsoleLogger : LogListenerBase
    {
        #region Methods
        protected override void Write(ILog log, string message, LogEvent logEvent, object? extraData, LogData? logData, DateTime time)
        {
            if (string.Equals(log.Tag, "Orc.CommandLine.CommandLineParser"))
            {
                return;
            }

            switch (logEvent)
            {
                case LogEvent.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case LogEvent.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.WriteLine("{0}", message);
        }
        #endregion
    }
}
