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
        public override DbDataReader GetRecords(DbQueryParameters queryParameters = null, int offset = 0, int fetchCount = -1)
        {
            var connection = GetOpenedConnection();
            var source = Source;
            var isPagingQuery = offset >= 0 && fetchCount >= 0;

            var sql = source.Table;
            DbCommand command;
            switch (source.TableType)
            {
                case TableType.Table:
                case TableType.View:
                {
                    var query = new Query(source.Table).Select();
                    if (isPagingQuery)
                    {
                        query = query.ForPage(offset / fetchCount + 1, fetchCount);
                    }

                    command = connection.CreateCommand(query);
                    break;
                }

                case TableType.StoredProcedure:
                    command = connection.CreateCommand(sql, CommandType.StoredProcedure);
                    break;

                case TableType.Function:
                    command = connection.CreateCommand($"select * from {source.Table}({queryParameters?.ToArgsNamesString() ?? string.Empty})");
                    break;

                case TableType.Sql:
                    command = connection.CreateCommand(sql);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            command.AddParameters(queryParameters);

            var reader = command.ExecuteReader();
            if (isPagingQuery && (source.TableType == TableType.Sql || source.TableType == TableType.StoredProcedure))
            {
                return new SkipTakeDbReader(reader, offset, fetchCount);
            }

            return reader;
        }

        public override long GetCount(DbQueryParameters queryParameters = null)
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
