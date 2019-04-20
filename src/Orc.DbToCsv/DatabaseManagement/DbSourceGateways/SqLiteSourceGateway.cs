// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqLiteSourceGateway.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using DataAccess;

    [ConnectToProvider("System.Data.SQLite")]
    public class SqLiteSourceGateway : SqlDbSourceGatewayBase
    {
        #region Constructors
        public SqLiteSourceGateway(DatabaseSource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        protected override Dictionary<TableType, Func<DbConnection, DbCommand>> GetObjectListCommandsFactory =>
            new Dictionary<TableType, Func<DbConnection, DbCommand>>
            {
                {TableType.Table, c => c.CreateCommand($"SELECT name FROM sqlite_master WHERE type ='table' AND name NOT LIKE 'sqlite_%';")},
                {TableType.View, c => c.CreateCommand($"SELECT name FROM sqlite_master WHERE type ='view' AND name NOT LIKE 'sqlite_%';")}
            };
        #endregion

        #region Methods
        public override DataSourceParameters GetQueryParameters()
        {
            return new DataSourceParameters();
        }
        #endregion
    }
}
