// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySqlSourceGateway.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using DataAccess;

    [ConnectToProvider("MySql.Data.MySqlClient")]
    public class MySqlSourceGateway : SqlDbSourceGatewayBase
    {
        #region Constructors
        public MySqlSourceGateway(DatabaseSource source) : base(source)
        {
        }
        #endregion

        #region Properties
        protected override Dictionary<TableType, Func<DbConnection, DbCommand>> GetObjectListCommandsFactory =>
            new Dictionary<TableType, Func<DbConnection, DbCommand>>
            {
                {TableType.Table, c => c.CreateCommand($"SELECT TABLE_NAME AS NAME FROM information_schema.tables where Table_schema = database() and Table_type = 'BASE TABLE';")},
                {TableType.View, c => c.CreateCommand($"SELECT TABLE_NAME AS NAME FROM information_schema.tables where Table_schema = database() and Table_type = 'VIEW';")},
                {TableType.StoredProcedure, c => c.CreateCommand($"SELECT SPECIFIC_NAME AS NAME FROM INFORMATION_SCHEMA.ROUTINES where ROUTINE_SCHEMA = database() and ROUTINE_TYPE = 'PROCEDURE';")},
            };

        protected override Dictionary<TableType, Func<DataSourceParameters>> DataSourceParametersFactory => new Dictionary<TableType, Func<DataSourceParameters>>
        {
            {TableType.StoredProcedure, () => GetArgs(GetArgsQuery)},
            {TableType.Function, () => GetArgs(GetArgsQuery)},
        };

        private string GetArgsQuery => $"SELECT PARAMETER_NAME AS NAME, DATA_TYPE AS TYPE FROM information_schema.parameters WHERE SPECIFIC_NAME = '{Source.Table}' and PARAMETER_MODE = 'IN';";
        #endregion

        #region Methods
        protected override DbCommand CreateGetTableRecordsCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount, bool isPagingEnabled)
        {
            var source = Source;
            var query = isPagingEnabled
                ? offset == 0
                    ? $"SELECT * FROM `{source.Table}` LIMIT {fetchCount}"
                    : $"SELECT * FROM `{source.Table}` LIMIT {fetchCount} OFFSET {offset}"
                : $"SELECT * FROM `{source.Table}`";

            return connection.CreateCommand(query);
        }

        protected override DbCommand CreateGetFunctionRecordsCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount)
        {
            throw new NotSupportedException("Table valued function in MySql not supported");
        }

        protected override DbCommand CreateTableCountCommand(DbConnection connection)
        {
            return connection.CreateCommand($"SELECT COUNT(*) AS `count` FROM `{Source.Table}`");
        }
        #endregion
    }
}
