// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlDbSourceGateway.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using DataAccess;

    [ConnectToProvider("Npgsql")]
    public class PostgreSqlDbSourceGateway : SqlDbSourceGatewayBase
    {
        #region Constructors
        public PostgreSqlDbSourceGateway(DatabaseSource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        protected override Dictionary<TableType, Func<DbConnection, DbCommand>> GetObjectListCommandsFactory =>
            new Dictionary<TableType, Func<DbConnection, DbCommand>>
            {
                {TableType.Table, c => c.CreateCommand($"SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';")},
                {TableType.View, c => c.CreateCommand($"SELECT table_name FROM information_schema.views WHERE table_schema = 'public';")},
                {
                    TableType.StoredProcedure, c => c.CreateCommand(@"SELECT  p.proname
                                FROM    pg_catalog.pg_namespace n
                                JOIN    pg_catalog.pg_proc p
                                ON      p.pronamespace = n.oid
                                WHERE   n.nspname = 'public';")
                },
                {
                    TableType.Function, c => c.CreateCommand(@"SELECT   distinct  r.routine_name
                                FROM     information_schema.routines r
                                JOIN     information_schema.parameters p
                                USING   (specific_catalog, specific_schema, specific_name)
                                JOIN     pg_namespace pg_n ON r.specific_schema = pg_n.nspname
                                JOIN     pg_proc pg_p ON pg_p.pronamespace = pg_n.oid
                                AND      pg_p.proname = r.routine_name
                                Where 	 r.data_type = 'record' AND pg_n.nspname = 'public'")
                },
            };
        #endregion

        #region Methods
        protected override DbCommand CreateGetTableRecordsCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount, bool isPagingEnabled)
        {
            var source = Source;
            var query = isPagingEnabled
                ? offset == 0
                    ? $"SELECT * FROM \"{source.Table}\" LIMIT {fetchCount}"
                    : $"SELECT * FROM \"{source.Table}\" LIMIT {fetchCount} OFFSET {offset}"
                : $"SELECT * FROM \"{source.Table}\"";

            return connection.CreateCommand(query);
        }

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
                    //TODO: parse sql string
                    break;

                case TableType.StoredProcedure:
                case TableType.Function:
                {
                    var query = $"SELECT pg_get_function_identity_arguments('{source.Table}'::regproc);";

                    var connection = GetOpenedConnection();
                    using (var reader = connection.GetReaderSql(query))
                    {
                        while (reader.Read())
                        {
                            var result = reader.GetString(0);
                            if (string.IsNullOrEmpty(result))
                            {
                                return new DataSourceParameters();
                            }

                            var parameters = result.Split(',').Select(x => x.Trim().Split(' ')).Select(x => new DataSourceParameter
                            {
                                Name = $"@{x[0]}",
                                Type = x[1]
                            }).ToList();

                            return new DataSourceParameters {Parameters = parameters};
                        }
                    }

                    return new DataSourceParameters();
                }

                default:
                    return new DataSourceParameters();
            }

            return new DataSourceParameters();
        }

        protected override DbCommand CreateGetStoredProcedureRecordsCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount)
        {
            return connection.CreateCommand($"call {Source.Table}({parameters?.ToArgsNamesString() ?? string.Empty})");
        }

        protected override DbCommand CreateTableCountCommand(DbConnection connection)
        {
            return connection.CreateCommand($"SELECT COUNT(*) AS \"count\" FROM \"{Source.Table}\"");
        }
        #endregion
    }
}
