// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsSqlDataBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Windows.Markup.Localizer;
    using SqlKata;

    [ConnectToProvider("System.Data.SqlClient")]
    public class MsSqlDbSourceGateway : DbSourceGateway
    {
        #region Constructors
        public MsSqlDbSourceGateway(DatabaseSource source)
            : base(source)
        {
        }
        #endregion

        #region Methods
        public override DbDataReader GetRecords(DbQueryParameters queryParameters = null, int offset = 0, int fetchCount = -1)
        {
            var connection = GetOpenedConnection();
            var source = Source;

            var sql = source.Table;
            DbCommand command;
            switch (source.TableType)
            {
                case TableType.Table:
                case TableType.View:
                {
                    var query = new Query(source.Table).Select("*");
                    if (offset <= 0 && fetchCount <= 0)
                    {
                        query = query.ForPage(offset / fetchCount + 1, fetchCount);
                    }

                    command = connection.CreateCommand(query);
                    break;
                }

                case TableType.StoredProcedure:
                    command = connection.CreateCommand(sql, CommandType.StoredProcedure);
                    break;

                case TableType.Sql:
                    command = connection.CreateCommand(sql);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            command.AddParameters(queryParameters);

            return command.ExecuteReader();
        }

        public override int GetCount(DbQueryParameters queryParameters = null)
        {
            var source = Source;
            var connection = GetOpenedConnection();

            switch (source.TableType)
            {
                case TableType.Table:
                case TableType.View:
                    return (int) connection.CreateCommand(new Query(source.Table).AsCount("*")).ExecuteScalar();

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

        public override DbQueryParameters GetQueryParameters()
        {
            var source = Source;
            switch (source.TableType)
            {
                case TableType.Sql:
                    return new DbQueryParameters();

                case TableType.StoredProcedure:
                {
                    var query = $"SELECT [name], type_name(user_type_id) as type FROM [sys].[parameters] WHERE [object_id] = object_id('{source.Table}')";

                    var connection = GetOpenedConnection();
                    var queryParameters = new DbQueryParameters();
                    using (var reader = connection.GetReader(query))
                    {
                        while (reader.Read())
                        {
                            var args = new DbQueryParameter
                            {
                                Name = (string)reader["Name"],
                                Type = (string)reader["Type"]
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

            return new DbQueryParameters();
        }

        public override IList<DbObject> GetObjects()
        {
            string literalType;
            var tableType = Source.TableType;
            switch (tableType)
            {
                case TableType.StoredProcedure:
                    literalType = "P";
                    break;

                case TableType.Table:
                    literalType = "U";
                    break;

                case TableType.View:
                    literalType = "V";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(tableType), tableType, null);
            }

            var connection = GetOpenedConnection();
            var query = new Query("dbo.sysobjects")
                .Select("name")
                .Where("uid", 1)
                .Where("type", literalType)
                .OrderBy("name");

            var dbObjects = new List<DbObject>();
            using (var reader = connection.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    var dbObject = new DbObject(tableType) { Name = reader.GetString(0) };

                    dbObjects.Add(dbObject);
                }
            }

            return dbObjects;
        }
        #endregion
    }
}
