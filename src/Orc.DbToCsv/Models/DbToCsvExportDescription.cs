// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbToCsvExportDescription.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using DataAccess;
    using DataAccess.Database;

    public class DbToCsvExportDescription
    {
        #region Properties
        public DatabaseSource Source { get; set; }
        public DataSourceParameters Parameters { get; set; }
        public string CsvFilePath { get; set; }
        #endregion
    }
}
