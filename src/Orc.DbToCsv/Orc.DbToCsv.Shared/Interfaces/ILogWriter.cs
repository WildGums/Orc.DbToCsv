// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogWriter.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    public interface ILogWriter
    {
        #region Methods
        void WriteLine(string message);
        #endregion
    }
}