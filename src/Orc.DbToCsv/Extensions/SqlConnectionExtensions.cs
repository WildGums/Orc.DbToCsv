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
    using Catel;
    using DataAccess;
    using DatabaseManagement;
    using SqlKata;

    internal static class SqlConnectionExtensions
    {
        #region Fields
        private static readonly Dictionary<Type, DbProvider> ConnectionTypeToProvider = new Dictionary<Type, DbProvider>();
        #endregion

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

            var command = connection.CreateCommand(sql, commandType, commandTimeout);
            return command.ExecuteReader();
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
            return GetProviderByConnectionType(connectionType);
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

        private static DbProvider GetProviderByConnectionType(Type connectionType)
        {
            Argument.IsNotNull(() => connectionType);

            if (ConnectionTypeToProvider.TryGetValue(connectionType, out var dbProvider))
            {
                return dbProvider;
            }

            var dbProviders = DbProvider.GetRegisteredProviders();
            foreach (var currentProvider in dbProviders.Values)
            {
                if (currentProvider.ConnectionType == connectionType)
                {
                    ConnectionTypeToProvider[connectionType] = currentProvider;
                    return currentProvider;
                }
            }

            return null;
        }
        #endregion
    }
}
