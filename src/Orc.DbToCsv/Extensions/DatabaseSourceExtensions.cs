// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseSourceExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Collections.Generic;
    using System.Data.Common;
    using Catel;
    using DatabaseManagement;

    public static class DatabaseSourceExtensions
    {
        #region Methods
        public static IList<DbObject> GetObjectsOfType(this DatabaseSource databaseSource, TableType tableType)
        {
            Argument.IsNotNull(() => databaseSource);

            var dataSourceCopy = new DatabaseSource(databaseSource.ToString())
            {
                TableType = tableType
            };

            var gateway = dataSourceCopy.CreateGateway();

            return gateway.GetObjects();
        }

        public static DbConnection CreateConnection(this DatabaseSource databaseSource)
        {
            Argument.IsNotNull(() => databaseSource);

            var provider = databaseSource.GetProvider();
            return provider?.CreateConnection(databaseSource);
        }

        public static DbSourceGatewayBase CreateGateway(this DatabaseSource databaseSource)
        {
            Argument.IsNotNull(() => databaseSource);

            var dbProvider = databaseSource.GetProvider();
            return dbProvider?.CreateDbSourceGateway(databaseSource);
        }

        public static DbProvider GetProvider(this DatabaseSource databaseSource)
        {
            Argument.IsNotNull(() => databaseSource);

            return DbProvider.GetRegisteredProvider(databaseSource.ProviderName);
        }
        #endregion
    }
}
