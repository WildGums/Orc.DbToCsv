// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlConnectionExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Common;
    using DatabaseManagement;
    using SqlKata;
    using SqlKata.Compilers;

    internal static class SqlConnectionExtensions
    {
        #region Methods
        public static async Task<T> OpenConnectionAsync<T>(this T connection, CancellationToken cancellationToken = default)
            where T : class, IDbConnection
        {
            Argument.IsNotNull(() => connection);

            if (!((object) connection is DbConnection dbConnection))
            {
                connection.Open();
                return connection;
            }
            await dbConnection.OpenAsync(cancellationToken);
            return connection;
        }

        public static IDataReader GetReaderSql(this IDbConnection connection, string sql, object parameters = null, int? commandTimeout = null)
        {
            Argument.IsNotNull(() => connection);

            return connection.GetReader(sql, CommandType.Text, parameters, commandTimeout);
        }

        public static IDataReader GetReader(this IDbConnection connection, string sql, CommandType commandType = CommandType.Text,
            object parameters = null, int? commandTimeout = null)
        {
            Argument.IsNotNull(() => connection);

            using (var command = connection.CreateCommand(sql, parameters, commandType, commandTimeout))
            {
                return command.ExecuteReader();
            }
        }

        //public static IDataReader GetReader(this DbConnection connection, Query query)
        //{
        //    var provider = connection.GetDbProvider();
        //    var compiler = provider.SqlCompiler;

        //    var result = compiler.Compile(query);

        //}

        public static IDbCommand CreateCommand(this IDbConnection connection, string sql, object parameters = null,
            CommandType commandType = CommandType.Text, int? commandTimeout = null)
        {
            Argument.IsNotNull(() => connection);

            var command = connection.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = sql;
            if (commandTimeout.HasValue)
            {
                command.CommandTimeout = commandTimeout.Value;
            }

            //command.AddParameters(parameters);
            return command;
        }

        internal static ISqlCompiler GetCompiler(this DbConnection connection)
        {
            Argument.IsNotNull(() => connection);

            var provider = connection.GetDbProvider();
            var sqlCompiler = provider.GetOrCreateConnectedInstance<ISqlCompiler>();

            return sqlCompiler;
        }

        public static DbProvider GetDbProvider(this DbConnection connection)
        {
            Argument.IsNotNull(() => connection);

            var connectionType = connection.GetType();
            return DbProviderCache.GetProviderByConnectionType(connectionType);
        }

        public static DbDataReader GetRecords(this DbConnection connection, DatabaseSource source, int offset, int fetchCount, DbQueryParameters queryParameters)
        {
            Argument.IsNotNull(() => connection);
            Argument.IsNotNull(() => source);

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var sql = source.Table;
            if (source.TableType == TableType.Table || source.TableType == TableType.View)
            {
                var query = new Query(source.Table)
                    .Select("*")
                    .ForPage(offset / fetchCount + 1, fetchCount);

                var compiler = connection.GetCompiler();
                sql = compiler.Compile(query);
            }

            return connection.GetReaderSql(sql) as DbDataReader;
        }

        internal static DbDataReader GetRecordsReader(this DbConnection connection, Project project, DatabaseSource source)
        {
            Argument.IsNotNull(() => connection);
            Argument.IsNotNull(() => project);
            Argument.IsNotNull(() => source);

            var query = new Query(source.Table)
                .Select("*")
                .ForPage(1, project.MaximumRowsInTable.Value);

            var compiler = connection.GetCompiler();
            var sql = compiler.Compile(query);

            return connection.GetReaderSql(sql) as DbDataReader;
        }

        internal static DbDataReader GetRecordsReader(this DbConnection connection, Project project, string tableName)
        {
            Argument.IsNotNull(() => connection);
            Argument.IsNotNull(() => project);

            var query = new Query(tableName)
                .Select("*")
                .ForPage(1, project.MaximumRowsInTable.Value);

            var compiler = connection.GetCompiler();
            var sql = compiler.Compile(query);

            return connection.GetReaderSql(sql) as DbDataReader;
        }

        internal static SqlCommand CreateGetAvailableTablesSqlCommand(this SqlConnection sqlConnection)
        {
            Argument.IsNotNull(() => sqlConnection);

            return new SqlCommand
            {
                CommandText = "SELECT '['+TABLE_SCHEMA+'].['+ TABLE_NAME + ']' FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_SCHEMA, TABLE_NAME",
                Connection = sqlConnection,
                CommandTimeout = 300
            };
        }
        #endregion
    }
}
