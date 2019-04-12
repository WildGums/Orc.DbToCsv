// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseSourceException.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    using System;

    public class DatabaseSourceException : Exception
    {
        #region Constructors
        public DatabaseSourceException(string message)
            : base(message)
        {
        }
        #endregion
    }
}
