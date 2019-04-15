// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Table.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    public class Table
    {
        #region Properties
        public string TableType { get; set; }
        public string Name { get; set; }
        public string Csv { get; set; }
        public string Output { get; set; }

        public string ConnectionString { get; set; }
        public string Provider { get; set; } = "System.Data.SqlClient";
        #endregion
    }
}
