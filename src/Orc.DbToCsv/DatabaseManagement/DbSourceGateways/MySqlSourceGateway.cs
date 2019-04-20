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
                    var query = $"SELECT PARAMETER_NAME AS NAME, DATA_TYPE AS TYPE FROM information_schema.parameters WHERE SPECIFIC_NAME = '{source.Table}' and PARAMETER_MODE = 'IN';";

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
            var source = Source;
            switch (source.TableType)
            {
                case TableType.Sql:
                    return new List<DbObject>();

                case TableType.Table:
                {
                    var sql = $"SELECT TABLE_NAME AS NAME FROM information_schema.tables where Table_schema = database() and Table_type = 'BASE TABLE';";
                    return ReadAllDbObjects(x => x.GetReaderSql(sql));
                }

                case TableType.View:
                {
                    var sql = $"SELECT TABLE_NAME AS NAME FROM information_schema.tables where Table_schema = database() and Table_type = 'VIEW';";
                    return ReadAllDbObjects(x => x.GetReaderSql(sql));
                }

                case TableType.StoredProcedure:
                {
                    var sql = $"SELECT SPECIFIC_NAME AS NAME FROM INFORMATION_SCHEMA.ROUTINES where ROUTINE_SCHEMA = database() and ROUTINE_TYPE = 'PROCEDURE';";

                    return ReadAllDbObjects(x => x.GetReaderSql(sql));
                }

                case TableType.Function:
                {
                    //Vladimir: MySQL has no functions returning table value
                    return new List<DbObject>();
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override DbCommand CreateFunctionCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount)
        {
            throw new NotSupportedException("Table valued function in MySql not supported");
        }
        #endregion
    }
}
