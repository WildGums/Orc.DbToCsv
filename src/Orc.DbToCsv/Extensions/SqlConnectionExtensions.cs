// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlConnectionExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Insight.Database;

    internal static class SqlConnectionExtensions
    {
        #region Methods
        public static async Task<T> OpenConnectionAsync1<T>(this T connection, CancellationToken cancellationToken = default (CancellationToken))
            where T : IDbConnection
        {
            var dbConnection = (object) (T) connection as DbConnection;
            if (dbConnection == null)
            {
                connection.Open();
                return connection;
            }
            await dbConnection.OpenAsync(cancellationToken);
            return connection;
        }

        public static IDataReader GetReaderSql1(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            return connection.GetReader1(sql, parameters, CommandType.Text, commandBehavior, commandTimeout, transaction);
        }

        public static IDataReader GetReader1(
            this IDbConnection connection,
            string sql,
            object parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            int? commandTimeout = null,
            IDbTransaction transaction = null)
        {
            using (var command = connection.CreateCommand(sql, parameters, commandType, commandTimeout, transaction))
                return command.ExecuteReader(commandBehavior);
        }

        internal static DbDataReader GetRecordsReader(this DbConnection connection, Project project, string tableName)
        {
            Argument.IsNotNull(() => connection);
            Argument.IsNotNull(() => project);

            return connection.GetReaderSql1($"select top {project.MaximumRowsInTable.Value} * from {tableName}") as DbDataReader;
        }

        internal static Task<IList<FastExpando>> GetRecordsAsync(this DbConnection connection, Project project, string tableName)
        {
            Argument.IsNotNull(() => connection);
            Argument.IsNotNull(() => project);

            return connection.QuerySqlAsync($"select top {project.MaximumRowsInTable.Value} * from {tableName}");
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
