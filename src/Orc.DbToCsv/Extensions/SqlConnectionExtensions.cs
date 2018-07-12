// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlConnectionExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Catel;

    internal static class SqlConnectionExtensions
    {
        #region Methods
        internal static SqlCommand CreateGetTableSchemaSqlCommand(this SqlConnection sqlConnection, string tableName)
        {
            Argument.IsNotNull(() => sqlConnection);
            Argument.IsNotNullOrEmpty(() => tableName);

            var command = new SqlCommand();
            command.Parameters.Add("@tableName", SqlDbType.VarChar).Value = tableName;
            command.CommandText = "SELECT c.name AS Name, t.name AS columnType " +
                                  "FROM sys.columns c INNER JOIN sys.types t ON c.system_type_id = t.system_type_id " +
                                  "WHERE c.object_id = OBJECT_ID(@tableName) and t.name<>'sysname'";

            command.Connection = sqlConnection;

            return command;
        }

        internal static SqlCommand CreateGetRecordsSqlCommand(this SqlConnection sqlConnection, string tableName, List<Tuple<string, string>> schema, int maximumRowsInTable)
        {
            Argument.IsNotNull(() => sqlConnection);
            Argument.IsNotNullOrEmpty(() => tableName);

            var command = new SqlCommand();

            var columns = string.Join("], [", schema.Select(t => t.Item1));
            var top = maximumRowsInTable > 0 ? $"TOP {maximumRowsInTable}" : string.Empty;
            command.CommandText = $"SELECT {top} [{columns}] FROM [{tableName}]";

            command.Connection = sqlConnection;
            command.CommandTimeout = 0;

            return command;
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
