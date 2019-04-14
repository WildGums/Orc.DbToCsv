// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbProviderCache.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.Common
{
    using System;
    using System.Collections.Generic;
    using Catel;
    using DatabaseManagement;

    internal static class DbProviderCache
    {
        #region Fields
        private static readonly Dictionary<Type, DbProvider> ConnectionTypeToProvider = new Dictionary<Type, DbProvider>();
        #endregion

        #region Methods

        public static DbProvider GetProviderByConnectionType(Type connectionType)
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
