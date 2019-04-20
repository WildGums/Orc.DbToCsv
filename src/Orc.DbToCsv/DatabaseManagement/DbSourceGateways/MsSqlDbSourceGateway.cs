// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsSqlDbSourceGateway.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using DataAccess;

    [ConnectToProvider("System.Data.SqlClient")]
    public class MsSqlDbSourceGateway : SqlDbSourceGatewayBase
    {
        #region Constructors
        public MsSqlDbSourceGateway(DatabaseSource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        protected override Dictionary<TableType, Func<DbConnection, DbCommand>> GetObjectListCommandsFactory =>
            new Dictionary<TableType, Func<DbConnection, DbCommand>>
            {
                {TableType.Table, c => CreateGetObjectsCommand(c, "U")},
                {TableType.View, c => CreateGetObjectsCommand(c, "V")},
                {TableType.StoredProcedure, c => CreateGetObjectsCommand(c, "P")},
                {TableType.Function, c => CreateGetObjectsCommand(c, "IF")},
            };

        protected override Dictionary<TableType, Func<DataSourceParameters>> DataSourceParametersFactory => new Dictionary<TableType, Func<DataSourceParameters>>
        {
            {TableType.StoredProcedure, () => GetArgs(GetArgsQuery)},
            {TableType.Function, () => GetArgs(GetArgsQuery)},
        };

        private string GetArgsQuery => $"SELECT [name], type_name(user_type_id) as type FROM [sys].[parameters] WHERE [object_id] = object_id('{Source.Table}')";
        #endregion

        #region Methods
        protected override DbCommand CreateGetTableRecordsCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount, bool isPagingEnabled)
        {
            var source = Source;
            var query = isPagingEnabled
                ? offset == 0
                    ? $"SELECT TOP ({fetchCount}) * FROM [{source.Table}]"
                    : $"SELECT * FROM (SELECT *, ROW_NUMBER() OVER (ORDER BY (SELECT 0)) AS [row_num] FROM [{source.Table}]) AS [results_wrapper] WHERE [row_num] BETWEEN {offset + 1} AND {offset + fetchCount}"
                : $"SELECT * FROM [{source.Table}]";

            return connection.CreateCommand(query);
        }

        protected override DbCommand CreateTableCountCommand(DbConnection connection)
        {
            return connection.CreateCommand($"SELECT COUNT(*) AS [count] FROM [{Source.Table}]");
        }

        private DbCommand CreateGetObjectsCommand(DbConnection connection, string commandParameter)
        {
            return connection.CreateCommand($"SELECT name FROM dbo.sysobjects WHERE uid = 1 AND type = '{commandParameter}' ORDER BY name;");
        }
        #endregion
    }
}
