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
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Insight.Database;

    internal static class SqlConnectionExtensions
    {
        #region Methods
        internal static IDataReader GetRecordsReader(this DbConnection connection, Project project, string tableName)
        {
            Argument.IsNotNull(() => connection);
            Argument.IsNotNull(() => project);

            return connection.GetReaderSql($"select top {project.MaximumRowsInTable.Value} * from {tableName}");
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
