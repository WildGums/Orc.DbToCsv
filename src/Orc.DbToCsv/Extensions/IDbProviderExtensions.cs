// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDbProviderExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv
{
    using System.Data;
    using System.Data.Common;
    using Catel;
    using DatabaseManagement;

    public static class IDbProviderExtensions
    {
        public static DbConnection CreateConnection(this DbProvider dbProvider, DatabaseSource databaseSource)
        {
            Argument.IsNotNull(() => dbProvider);
            Argument.IsNotNull(() => databaseSource);

            return CreateConnection(dbProvider, databaseSource.ConnectionString);
        }

        public static DbConnection CreateConnection(this DbProvider dbProvider, string connectionString)
        {
            Argument.IsNotNull(() => dbProvider);

            var connection = dbProvider.CreateConnection();
            if (connection == null)
            {
                return null;
            }

            connection.ConnectionString = connectionString; //.DecryptConnectionString(ProviderInvariantName);

            return connection;
        }
    }
}
