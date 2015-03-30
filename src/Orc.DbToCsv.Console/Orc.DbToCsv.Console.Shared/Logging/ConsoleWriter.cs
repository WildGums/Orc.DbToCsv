// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleWriter.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    public class ConsoleWriter : ILogWriter
    {
        #region Methods

        #region ILogWriter Members
        public void WriteLine(string message)
        {
            System.Console.WriteLine(message);
        }
        #endregion

        #endregion
    }
}