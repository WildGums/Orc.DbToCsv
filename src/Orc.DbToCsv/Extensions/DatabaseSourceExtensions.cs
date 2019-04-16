// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseSourceExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Data.Common;
    using Catel;
    using DatabaseManagement;

    public static class DatabaseSourceExtensions
    {
        #region Methods
        public static DbConnection CreateConnection(this DatabaseSource databaseSource)
        {
            Argument.IsNotNull(() => databaseSource);

            var provider = databaseSource.GetProvider();
            return provider?.CreateConnection(databaseSource);
        }

        public static DbSourceGateway CreateGateway(this DatabaseSource databaseSource)
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
