// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirebirdSourceGateway.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using DataAccess;

    [ConnectToProvider("FirebirdSql.Data.FirebirdClient")]
    public class FirebirdSourceGateway : SqlDbSourceGatewayBase
    {
        #region Constructors
        public FirebirdSourceGateway(DatabaseSource source) : base(source)
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
                {
                    var query = $"SELECT rdb$parameter_name, rdb$parameter_type FROM rdb$procedure_parameters WHERE rdb$procedure_name = '{Source.Table}'";

                    var connection = GetOpenedConnection();
                    var queryParameters = new DataSourceParameters();
                    using (var reader = connection.GetReader(query))
                    {
                        while (reader.Read())
                        {
                            var args = new DataSourceParameter
                            {
                                Name = reader.GetString(0),
                                Type = reader.GetValue(1)?.ToString()
                            };

                            queryParameters.Parameters.Add(args);
                        }
                    }

                    return queryParameters;
                }
                case TableType.Function:
                {
                    var query = $"SELECT rdb$argument_name, rdb$field_type FROM rdb$procedure_parameters WHERE rdb$procedure_name = '{Source.Table}'";

                    var connection = GetOpenedConnection();
                    var queryParameters = new DataSourceParameters();
                    using (var reader = connection.GetReader(query))
                    {
                        while (reader.Read())
                        {
                            var args = new DataSourceParameter
                            {
                                Name = reader.GetString(0),
                                Type = reader.GetValue(1)?.ToString()
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
            var source = Source;
            switch (source.TableType)
            {
                case TableType.Sql:
                    return new List<DbObject>();

                case TableType.Table:
                {
                    var sql = $"SELECT rdb$relation_name FROM rdb$relations WHERE rdb$view_blr is null AND (rdb$system_flag is null OR rdb$system_flag = 0);";
                    return ReadAllDbObjects(x => x.GetReaderSql(sql));
                }

                case TableType.View:
                {
                    var sql = $"SELECT rdb$relation_name FROM rdb$relations WHERE rdb$view_blr is not null AND (rdb$system_flag is null OR rdb$system_flag = 0);";
                    return ReadAllDbObjects(x => x.GetReaderSql(sql));
                }

                case TableType.StoredProcedure:
                {
                    var sql = $"SELECT rdb$procedure_name FROM rdb$procedures;";
                    return ReadAllDbObjects(x => x.GetReaderSql(sql));
                }

                case TableType.Function:
                {
                    var sql = $"SELECT rdb$function_name FROM rdb$functions;";
                    return ReadAllDbObjects(x => x.GetReaderSql(sql));
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion
    }
}
