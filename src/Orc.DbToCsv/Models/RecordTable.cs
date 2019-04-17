// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordTable.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System.Collections.Generic;

    public class RecordTable : List<Record>
    {
        #region Properties
        public string[] Headers { get; set; }
        #endregion
    }
}
