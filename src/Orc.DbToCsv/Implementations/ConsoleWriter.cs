namespace DbToCsv.Implementations
{
    using Orc.DbToCsv.Interfaces;

    public class ConsoleWriter : ILogWriter
    {
        public void WriteLine(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
