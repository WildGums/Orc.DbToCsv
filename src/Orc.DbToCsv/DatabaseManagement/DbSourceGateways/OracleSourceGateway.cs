// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OracleSourceGateway.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using DataAccess;

    [ConnectToProvider("Oracle.ManagedDataAccess.Client")]
    public class OracleSourceGateway : SqlDbSourceGatewayBase
    {
        #region Constructors
        public OracleSourceGateway(DatabaseSource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        protected override Dictionary<TableType, Func<DbConnection, DbCommand>> GetObjectListCommandsFactory =>
            new Dictionary<TableType, Func<DbConnection, DbCommand>>
            {
                {TableType.Table, c => c.CreateCommand($"SELECT table_name FROM user_tables")},
                {TableType.View, c => c.CreateCommand($"SELECT view_name from user_views")},
                {TableType.StoredProcedure, c => c.CreateCommand($"SELECT * FROM User_Procedures WHERE OBJECT_TYPE = 'PROCEDURE'")},
                {TableType.Function, c => c.CreateCommand($"SELECT * FROM User_Procedures WHERE OBJECT_TYPE = 'FUNCTION'")},
            };

        protected override Dictionary<TableType, Func<DataSourceParameters>> DataSourceParametersFactory => new Dictionary<TableType, Func<DataSourceParameters>>
        {
            {TableType.StoredProcedure, () => GetArgs(GetArgsQuery)},
            {TableType.Function, () => GetArgs(GetArgsQuery)},
        };

        private string GetArgsQuery => $"SELECT ARGUMENT_NAME AS NAME, DATA_TYPE AS TYPE FROM USER_ARGUMENTS WHERE OBJECT_NAME = UPPER('{Source.Table}') AND IN_OUT = 'IN'";
        #endregion

        #region Methods
        public override DataSourceParameters GetQueryParameters()
        {
            var source = Source;
            switch (source.TableType)
            {
                case TableType.Table:
                    break;
                case TableType.View:
                    break;
                case TableType.Sql:
                    return new DataSourceParameters();

                case TableType.StoredProcedure:
                case TableType.Function:
                {
                    var query = $"SELECT ARGUMENT_NAME AS NAME, DATA_TYPE AS TYPE FROM USER_ARGUMENTS WHERE OBJECT_NAME = UPPER('{Source.Table}') AND IN_OUT = 'IN'";
                    return GetArgs(query);
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new DataSourceParameters();
        }

        protected override DbCommand CreateGetTableRecordsCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount, bool isPagingEnabled)
        {
            var sql = $"SELECT * FROM {Source.Table}";
            if (isPagingEnabled)
            {
                sql = $"SELECT * FROM {Source.Table} ORDER BY (select COLUMN_NAME from ALL_TAB_COLUMNS where TABLE_NAME='{Source.Table}' FETCH FIRST 1 ROWS ONLY) OFFSET {offset} ROWS FETCH NEXT {fetchCount} ROWS ONLY";
            }

            return connection.CreateCommand(sql);
        }

        protected override DbCommand CreateGetStoredProcedureRecordsCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount)
        {
            return connection.CreateCommand(Source.Table, CommandType.StoredProcedure);
        }

        protected override DbCommand CreateGetFunctionRecordsCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount)
        {
            return connection.CreateCommand($"SELECT * FROM table({Source.Table}({parameters?.ToArgsNamesString(":") ?? string.Empty}))");
        }

        protected override DbCommand CreateTableCountCommand(DbConnection connection)
        {
            return connection.CreateCommand($"SELECT COUNT(*) \"count\" FROM \"{Source.Table}\"");
        }
        #endregion
    }
}
