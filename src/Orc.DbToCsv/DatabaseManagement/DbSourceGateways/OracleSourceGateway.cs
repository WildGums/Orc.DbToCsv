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
    using SqlKata;

    [ConnectToProvider("Oracle.ManagedDataAccess.Client")]
    public class OracleSourceGateway : SqlDbSourceGatewayBase
    {
        #region Constructors
        public OracleSourceGateway(DatabaseSource source) 
            : base(source)
        {
        }
        #endregion

        #region Methods
        public override DataSourceParameters GetQueryParameters()
        {
            var source = Source;
            switch (source.TableType)
            {
                case TableType.Sql:
                    return new DataSourceParameters();

                case TableType.StoredProcedure:
                case TableType.Function:
                {
                    var query = $"SELECT ARGUMENT_NAME AS NAME, DATA_TYPE AS TYPE FROM USER_ARGUMENTS WHERE OBJECT_NAME = UPPER('{Source.Table}') AND IN_OUT = 'IN'";

                    var connection = GetOpenedConnection();
                    var queryParameters = new DataSourceParameters();
                    using (var reader = connection.GetReader(query))
                    {
                        while (reader.Read())
                        {
                            var args = new DataSourceParameter
                            {
                                Name = reader.GetString(0),
                                Type = reader.GetString(1)
                            };

                            queryParameters.Parameters.Add(args);
                        }
                    }

                    return queryParameters;
                }

                case TableType.Table:
                    break;

                case TableType.View:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new DataSourceParameters();
        }

        public override IList<DbObject> GetObjects()
        {
            return new List<DbObject>();
        }

        protected override DbCommand CreateTableCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount)
        {
            var isPagingQuery = offset >= 0 && fetchCount >= 0;
            var sql = $"SELECT * FROM {Source.Table}";
            if (isPagingQuery)
            {
                sql = $"SELECT * FROM {Source.Table} ORDER BY (select COLUMN_NAME from ALL_TAB_COLUMNS where TABLE_NAME='{Source.Table}' FETCH FIRST 1 ROWS ONLY) OFFSET {offset} ROWS FETCH NEXT {fetchCount} ROWS ONLY";
            }

            return connection.CreateCommand(sql);
        }

        protected override DbCommand CreateStoredProcedureCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount)
        {
            return connection.CreateCommand(Source.Table, CommandType.StoredProcedure);
        }

        protected override DbCommand CreateFunctionCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount)
        {
            return connection.CreateCommand($"select * from table({Source.Table}({parameters?.ToArgsNamesString(":") ?? string.Empty}))");
        }
        #endregion
    }
}
