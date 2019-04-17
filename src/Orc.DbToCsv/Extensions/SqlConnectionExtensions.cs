﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlConnectionExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Data;
    using System.Data.Common;
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
                return command.ExecuteReader();
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
        #endregion
    }
}
