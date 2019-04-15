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
        public static DbProvider GetProvider(this DatabaseSource databaseSource)
        {
            Argument.IsNotNull(() => databaseSource);

            return DbProvider.GetRegisteredProviders()[databaseSource.ProviderName];
        }

        public static DbConnection CreateConnection(this DatabaseSource databaseSource)
        {
            Argument.IsNotNull(() => databaseSource);

            var provider = databaseSource.GetProvider();
            return provider?.CreateConnection(databaseSource);
        }
        #endregion
    }
}
