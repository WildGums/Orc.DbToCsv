// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlDbSourceGateway.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [ConnectToProvider("Npgsql")]
    public class PostgreSqlDbSourceGateway : SqlDbSourceGatewayBase
    {
        #region Constructors
        public PostgreSqlDbSourceGateway(DatabaseSource source)
            : base(source)
        {
        }
        #endregion

        #region Methods
        public override DbQueryParameters GetQueryParameters()
        {
            var source = Source;
            if (source.TableType == TableType.StoredProcedure)
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
                            return new DbQueryParameters();
                        }

                        var parameters = result.Split(',').Select(x => x.Trim().Split(' ')).Select(x => new DbQueryParameter
                        {
                            Name = x[0],
                            Type =  x[1]
                        }).ToList();

                        return new DbQueryParameters { Parameters = parameters};
                    }
                }

                return new DbQueryParameters();
            }

            return new DbQueryParameters();
        }

        public override IList<DbObject> GetObjects()
        {
            var source = Source;
            switch (source.TableType)
            {
                case TableType.Sql:
                    return new List<DbObject>();

                case TableType.Table:
                {
                    var sql = "SELECT * FROM information_schema.tables WHERE table_schema = 'public';";
                    return ReadAllDbObjects(x => x.GetReaderSql(sql));
                }

                case TableType.View:
                {
                    var sql = "SELECT * FROM information_schema.views WHERE table_schema = 'public';";
                    return ReadAllDbObjects(x => x.GetReaderSql(sql));
                }

                case TableType.StoredProcedure:
                {
                    var sql = @"SELECT  p.proname
                                FROM    pg_catalog.pg_namespace n
                                JOIN    pg_catalog.pg_proc p
                                ON      p.pronamespace = n.oid
                                WHERE   n.nspname = 'public';";

                    return ReadAllDbObjects(x => x.GetReaderSql(sql));
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion
    }
}
