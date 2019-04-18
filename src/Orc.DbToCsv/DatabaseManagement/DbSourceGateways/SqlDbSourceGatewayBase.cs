// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlDbSourceGatewayBase.cs" company="WildGums">
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

    public abstract class SqlDbSourceGatewayBase : DbSourceGatewayBase
    {
        #region Constructors
        protected SqlDbSourceGatewayBase(DatabaseSource source)
            : base(source)
        {
        }
        #endregion

        #region Methods
        public override DbDataReader GetRecords(DataSourceParameters queryParameters = null, int offset = 0, int fetchCount = -1)
        {
            var connection = GetOpenedConnection();
            var source = Source;
            var isPagingQuery = offset >= 0 && fetchCount >= 0;

            var sql = source.Table;
            DbCommand command;
            switch (source.TableType)
            {
                case TableType.Table:
                    command = CreateTableCommand(connection, queryParameters, offset, fetchCount);
                    break;

                case TableType.View:
                    command = CreateViewCommand(connection, queryParameters, offset, fetchCount);
                    break;

                case TableType.StoredProcedure:
                    command = CreateStoredProcedureCommand(connection, queryParameters, offset, fetchCount);
                    break;

                case TableType.Function:
                    command = CreateFunctionCommand(connection, queryParameters, offset, fetchCount);
                    break;

                case TableType.Sql:
                    command = connection.CreateCommand(sql);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            command.AddParameters(queryParameters);

            var reader = command.ExecuteReader();
            if (isPagingQuery && (source.TableType == TableType.Sql || source.TableType == TableType.StoredProcedure || source.TableType == TableType.Function))
            {
                return new SkipTakeDbReader(reader, offset, fetchCount);
            }

            return reader;
        }

        protected virtual DbCommand CreateTableCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount)
        {
            var isPagingQuery = offset >= 0 && fetchCount >= 0;

            var query = new Query(Source.Table).Select();
            if (isPagingQuery)
            {
                query = query.ForPage(offset / fetchCount + 1, fetchCount);
            }

            return connection.CreateCommand(query);
        }

        protected virtual DbCommand CreateViewCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount)
        {
            return CreateTableCommand(connection, parameters, offset, fetchCount);
        }

        protected virtual DbCommand CreateStoredProcedureCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount)
        {
            return connection.CreateCommand(Source.Table, CommandType.StoredProcedure);
        }

        protected virtual DbCommand CreateFunctionCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount)
        {
            return connection.CreateCommand($"select * from {Source.Table}({parameters?.ToArgsNamesString() ?? string.Empty})");
        }

        public override long GetCount(DataSourceParameters queryParameters = null)
        {
            var source = Source;
            var connection = GetOpenedConnection();

            switch (source.TableType)
            {
                case TableType.Table:
                case TableType.View:
                    var res = connection.CreateCommand(new Query(source.Table).AsCount()).ExecuteScalar();
                    return (long)res;

                case TableType.StoredProcedure:
                {
                    var command = connection.CreateCommand(source.Table, CommandType.StoredProcedure);
                    command.AddParameters(queryParameters);

                    var count = 0;
                    using (var reader = command.ExecuteReader())
                    {
                        while (true)
                        {
                            while (reader.Read())
                            {
                                count++;
                            }

                            if (!reader.NextResult())
                            {
                                break;
                            }

                            if (!reader.HasRows)
                            {
                                break;
                            }
                        }
                    }

                    return count;
                }
                default:
                    return 0;
            }
        }

        protected virtual IList<DbObject> ReadAllDbObjects(Func<DbConnection, DbDataReader> createReader, DbConnection connection)
        {
            var dbObjects = new List<DbObject>();
            var tableType = Source.TableType;
            using (var reader = createReader(connection))
            {
                while (reader.Read())
                {
                    var dbObject = new DbObject(tableType) {Name = reader.GetString(0)};
                    dbObjects.Add(dbObject);
                }
            }

            return dbObjects;
        }
        #endregion
    }
}
