// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReader.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DataAccess
{
    using System;
    using System.Globalization;

    public interface IReader : IDisposable
    {
        #region Properties
        string[] FieldHeaders { get; }
        string this[int index] { get; }
        string this[string name] { get; }
        int TotalRecordCount { get; }

        CultureInfo Culture { get; set; }
        int Offset { get; set; }
        int FetchCount { get; set; }
        #endregion

        #region Methods
        bool Read();
        #endregion
    }
}
