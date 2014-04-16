namespace Orc.DbToCsv.Console.Implementations
{
    using Orc.DbToCsv.Core.Interfaces;

    public class ConsoleWriter : ILogWriter
    {
        public void WriteLine(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
