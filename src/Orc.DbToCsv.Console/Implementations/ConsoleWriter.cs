// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleWriter.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2014 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.Console.Implementations
{
    public class ConsoleWriter : ILogWriter
    {
        #region ILogWriter Members
        public void WriteLine(string message)
        {
            System.Console.WriteLine(message);
        }
        #endregion
    }
}