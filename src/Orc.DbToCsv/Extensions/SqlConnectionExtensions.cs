// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlConnectionExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Common;
    using DatabaseManagement;
    using SqlKata;

    internal static class SqlConnectionExtensions
    {
        #region Methods
        public static IDataReader GetReaderSql(this DbConnection connection, string sql, object parameters = null, int? commandTimeout = null)
        {
            Argument.IsNotNull(() => connection);

            return connection.GetReader(sql, CommandType.Text, parameters, commandTimeout);
        }

        public static IDataReader GetReader(this DbConnection connection, string sql, CommandType commandType = CommandType.Text,
            object parameters = null, int? commandTimeout = null)
        {
            Argument.IsNotNull(() => connection);

            using (var command = connection.CreateCommand(sql, parameters, commandType, commandTimeout))
            {
                return command.ExecuteReader();
            }
        }

        public static IDbCommand CreateCommand(this DbConnection connection, string sql, object parameters = null,
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
