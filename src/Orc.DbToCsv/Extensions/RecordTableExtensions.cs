// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordTableExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Linq;
    using Catel;
    using DataAccess;

    public static class RecordTableExtensions
    {
        #region Methods
        public static bool HasHeaders(this RecordTable table)
        {
            Argument.IsNotNull(() => table);

            return table.Headers?.Any() ?? false;
        }
        #endregion
    }
}
