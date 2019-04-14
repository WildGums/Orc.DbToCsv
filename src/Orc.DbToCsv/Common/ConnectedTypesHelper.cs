// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectedTypesHelper.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Caching;
    using Catel.IoC;
    using Catel.Reflection;

    internal static class ConnectedTypesHelper
    {
        #region Constants
        private static readonly CacheStorage<string, CacheStorage<Type, IList<Type>>> ConnectedTypes = new CacheStorage<string, CacheStorage<Type, IList<Type>>>();
        private static readonly CacheStorage<string, object> ConnectedInstances = new CacheStorage<string, object>();
        #endregion

        #region Methods
        public static T GetOrCreateConnectedInstance<T>(this DbProvider dbProvider)
        {
            Argument.IsNotNull(() => dbProvider);

            return (T)ConnectedInstances.GetFromCacheOrFetch(dbProvider.ProviderInvariantName, () => CreateConnectedInstance<T>(dbProvider));
        }

        public static T CreateConnectedInstance<T>(this DbProvider dbProvider)
        {
            var connectedType = dbProvider.GetConnectedTypes<T>().FirstOrDefault();
            if (connectedType == null)
            {
                return default;
            }

            var typeFactory = dbProvider.GetTypeFactory();
            return (T)typeFactory.CreateInstanceWithParametersAndAutoCompletion(connectedType);
        }

        public static IList<Type> GetConnectedTypes<T>(this DbProvider provider)
        {
            Argument.IsNotNull(() => provider);

            var connectedTypesCache = ConnectedTypes.GetFromCacheOrFetch(provider.ProviderInvariantName, () => new CacheStorage<Type, IList<Type>>());
            return connectedTypesCache.GetFromCacheOrFetch(typeof(T), () => provider.FindConnectedTypes<T>().ToList());
        }

        private static IEnumerable<Type> FindConnectedTypes<T>(this DbProvider provider)
        {
            var providerInvariantName = provider.ProviderInvariantName;
            var attributedSqlCompilerTypes = typeof(T).GetAllAssignableFrom();
            foreach (var attributedSqlCompilerType in attributedSqlCompilerTypes)
            {
                var connectToProviderAttribute = attributedSqlCompilerType.GetCustomAttributeEx(typeof(ConnectToProviderAttribute), true) as ConnectToProviderAttribute;
                if (connectToProviderAttribute == null)
                {
                    continue;
                }

                if (connectToProviderAttribute.ProviderInvariantName == providerInvariantName)
                {
                    yield return attributedSqlCompilerType;
                }
            }
        }
        #endregion
    }
}
