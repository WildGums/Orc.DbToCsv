// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReader.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DataAccess
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Catel.Data;

    public interface IReader : IDisposable
    {
        #region Properties
        IValidationContext ValidationContext { get; }
        string[] FieldHeaders { get; }
        object this[int index] { get; }
        object this[string name] { get; }
        int TotalRecordCount { get; }
        CultureInfo Culture { get; set; }
        int Offset { get; set; }
        int FetchCount { get; set; }
        #endregion

        #region Methods
        bool Read();
        Task<bool> NextResultAsync();
        #endregion
    }
}
