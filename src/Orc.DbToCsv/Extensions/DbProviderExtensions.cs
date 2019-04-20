// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbProviderExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using Catel;
    using Catel.Caching;
    using Catel.IoC;
    using Catel.Reflection;
    using DatabaseManagement;

    public static class DbProviderExtensions
    {
        #region Fields
        private static readonly CacheStorage<string, CacheStorage<Type, IList<Type>>> ConnectedTypes = new CacheStorage<string, CacheStorage<Type, IList<Type>>>();
        private static readonly CacheStorage<string, object> ConnectedInstances = new CacheStorage<string, object>();
        #endregion

        #region Methods
        public static T GetOrCreateConnectedInstance<T>(this DbProvider dbProvider)
        {
            Argument.IsNotNull(() => dbProvider);

            return (T)ConnectedInstances.GetFromCacheOrFetch(dbProvider.ProviderInvariantName, () => CreateConnectedInstance<T>(dbProvider));
        }

        public static T CreateConnectedInstance<T>(this DbProvider dbProvider, params object[] parameters)
        {
            Argument.IsNotNull(() => dbProvider);

            var connectedType = dbProvider.GetConnectedTypes<T>().FirstOrDefault();
            if (connectedType == null)
            {
                return default;
            }

            var typeFactory = dbProvider.GetTypeFactory();
            return (T)typeFactory.CreateInstanceWithParametersAndAutoCompletion(connectedType, parameters);
        }

        public static IList<Type> GetConnectedTypes<T>(this DbProvider provider)
        {
            Argument.IsNotNull(() => provider);

            var connectedTypesCache = ConnectedTypes.GetFromCacheOrFetch(provider.ProviderInvariantName, () => new CacheStorage<Type, IList<Type>>());
            return connectedTypesCache.GetFromCacheOrFetch(typeof(T), () => provider.FindConnectedTypes<T>().ToList());
        }

        public static void ConnectType<TBaseType>(this DbProvider provider, Type type)
        {
            Argument.IsNotNull(() => provider);
            Argument.IsNotNull(() => type);

            var connectedTypesCache = ConnectedTypes.GetFromCacheOrFetch(provider.ProviderInvariantName, () => new CacheStorage<Type, IList<Type>>());
            var types = connectedTypesCache.GetFromCacheOrFetch(typeof(TBaseType), () => provider.FindConnectedTypes<TBaseType>().ToList());
            if (!types.Contains(type))
            {
                types.Add(type);
            }
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

        public static DbConnection CreateConnection(this DbProvider dbProvider, DatabaseSource databaseSource)
        {
            Argument.IsNotNull(() => dbProvider);
            Argument.IsNotNull(() => databaseSource);

            return CreateConnection(dbProvider, databaseSource.ConnectionString);
        }

        public static DbSourceGatewayBase CreateDbSourceGateway(this DbProvider dbProvider, DatabaseSource databaseSource)
        {
            Argument.IsNotNull(() => databaseSource);

            return dbProvider.CreateConnectedInstance<DbSourceGatewayBase>(databaseSource);
        }

        public static DbConnection CreateConnection(this DbProvider dbProvider, string connectionString)
        {
            Argument.IsNotNull(() => dbProvider);

            var connection = dbProvider.CreateConnection();
            if (connection == null)
            {
                return null;
            }

            connection.ConnectionString = connectionString.DecryptConnectionString(dbProvider.ProviderInvariantName);

            return connection;
        }
        #endregion
    }
}
