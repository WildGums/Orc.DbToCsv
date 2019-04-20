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
    using Catel;
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

        protected virtual DbCommand CreateTableCountCommand(DbConnection connection)
        {
            return connection.CreateCommand(new Query(Source.Table).AsCount());
        }

        protected virtual DbCommand CreateViewCountCommand(DbConnection connection)
        {
            return CreateTableCountCommand(connection);
        }

        public override long GetCount(DataSourceParameters queryParameters = null)
        {
            var source = Source;
            var connection = GetOpenedConnection();

            switch (source.TableType)
            {
                case TableType.Table:
                {
                    return Convert.ToInt64( CreateTableCountCommand(connection).ExecuteScalar() );
                }

                case TableType.View:
                {
                    return Convert.ToInt64( CreateViewCountCommand(connection).ExecuteScalar() );
                }

                case TableType.Function:
                {
                    var command = CreateFunctionCommand(connection, queryParameters, -1, -1);
                    command.AddParameters(queryParameters);
                    var count = command.GetRecordsCount();

                    return count;
                }

                case TableType.StoredProcedure:
                {
                    var command = CreateStoredProcedureCommand(connection, queryParameters, -1, -1);
                    command.AddParameters(queryParameters);
                    var count = command.GetRecordsCount();

                    return count;
                }

                case TableType.Sql:
                {
                    var command = connection.CreateCommand(source.Table);
                    command.AddParameters(queryParameters);
                    var count = command.GetRecordsCount();

                    return count;
                }

                default:
                {
                    return 0;
                }
            }
        }

        protected virtual IList<DbObject> ReadAllDbObjects(Func<DbConnection, DbDataReader> createReader)
        {
            var dbObjects = new List<DbObject>();
            var connection = GetOpenedConnection();
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
