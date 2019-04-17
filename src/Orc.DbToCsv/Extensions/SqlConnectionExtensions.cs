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
    using Catel;
    using Common;
    using DataAccess;
    using DatabaseManagement;
    using SqlKata;

    internal static class SqlConnectionExtensions
    {
        #region Methods
        public static DbDataReader GetReaderSql(this DbConnection connection, string sql, int? commandTimeout = null)
        {
            Argument.IsNotNull(() => connection);

            return connection.GetReader(sql, CommandType.Text, commandTimeout);
        }

        public static DbDataReader GetReader(this DbConnection connection, string sql, CommandType commandType = CommandType.Text,
            int? commandTimeout = null)
        {
            Argument.IsNotNull(() => connection);

            using (var command = connection.CreateCommand(sql, commandType, commandTimeout))
            {
                return command.ExecuteReader() as DbDataReader;
            }
        }

        public static DbCommand CreateCommand(this DbConnection connection, string sql,
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

        public static DbDataReader GetRecords(this DbConnection connection, DatabaseSource source, int offset, int fetchCount, DataSourceParameters queryParameters)
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

                return connection.ExecuteReader(query) as DbDataReader;
            }

            return connection.GetReaderSql(sql) as DbDataReader;
        }

        internal static DbCommand CreateCommand(this DbConnection connection, Query query, int? commandTimeout = null)
        {
            Argument.IsNotNull(() => connection);
            Argument.IsNotNull(() => query);

            var compiler = connection.GetCompiler();
            var sql = compiler.Compile(query);
            return connection.CreateCommand(sql, CommandType.Text, commandTimeout);
        }

        internal static DbDataReader ExecuteReader(this DbConnection connection, Query query, int? commandTimeout = null)
        {
            Argument.IsNotNull(() => connection);
            Argument.IsNotNull(() => query);

            var compiler = connection.GetCompiler();
            var sql = compiler.Compile(query);
            return connection.GetReaderSql(sql, commandTimeout);
        }

        public static DbCommand AddParameters(this DbCommand dbCommand, DataSourceParameters parameters)
        {
            Argument.IsNotNull(() => dbCommand);

            parameters?.Parameters?.ForEach(x => dbCommand.AddParameter(x));

            return dbCommand;
        }

        public static DbCommand AddParameter(this DbCommand dbCommand, DataSourceParameter parameter)
        {
            Argument.IsNotNull(() => dbCommand);
            Argument.IsNotNull(() => parameter);

            return dbCommand.AddParameter(parameter.Name, parameter.Value);
        }

        public static DbCommand AddParameter(this DbCommand dbCommand, string name, object value)
        {
            Argument.IsNotNull(() => dbCommand);

            var parameter = dbCommand.CreateParameter();
            parameter.Value = value;
            parameter.ParameterName = name;
            dbCommand.Parameters.Add(parameter);

            return dbCommand;
        }

        public static DbCommand CreateCommandFromQuery(this DbConnection connection, string query)
        {
            Argument.IsNotNull(() => connection);

            var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            return command;
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
