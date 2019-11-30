// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Table.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Collections.Generic;

    public class Table
    {
        #region Constructors
        public Table()
        {
            Parameters = new List<Parameter>();
        }
        #endregion

        #region Properties
        public string Schema { get; set; }
        public string TableType { get; set; }
        public string Name { get; set; }
        public string Csv { get; set; }
        public string Output { get; set; }
        public string ConnectionString { get; set; }
        public string Provider { get; set; } = "System.Data.SqlClient";
        public List<Parameter> Parameters { get; set; }
        #endregion
    }
}
