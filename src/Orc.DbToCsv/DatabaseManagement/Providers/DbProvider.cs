// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using Catel;
    using Catel.Collections;
    using Common;

    public class DbProvider
    {
        #region Constants
        private static readonly Dictionary<string, DbProvider> Providers = new Dictionary<string, DbProvider>();
        private static bool IsProvidersInitialized = false;
        #endregion

        #region Fields
        private Type _connectionType;
        private DbProviderFactory _dbProviderFactory;
        private DbProviderInfo _info;
        #endregion

        #region Constructors
        public DbProvider(DbProviderInfo info)
            : this(info.InvariantName)
        {
            Argument.IsNotNull(() => info);

            _info = info;
        }

        public DbProvider(string providerInvariantName)
        {
            Argument.IsNotNullOrWhitespace(() => providerInvariantName);

            ProviderInvariantName = providerInvariantName;
        }
        #endregion

        #region Properties
        protected DbProviderFactory DbProviderFactory => _dbProviderFactory ?? (_dbProviderFactory = DbProviderFactories.GetFactory(ProviderInvariantName));
        public virtual Type ConnectionType => _connectionType ?? (_connectionType = DbProviderFactory.CreateConnection()?.GetType());
        public virtual DbProviderInfo Info => GetInfo();
        public string Dialect { get; }
        public string ProviderInvariantName { get; }
        #endregion

        #region Methods
        public static void RegisterCustomProvider(DbProvider provider)
        {
            Argument.IsNotNull(() => provider);

            Providers[provider.ProviderInvariantName] = provider;
        }

        public static IReadOnlyDictionary<string, DbProvider> GetRegisteredProviders()
        {
            var providers = Providers;
            if (!IsProvidersInitialized)
            {
                DbProviderFactories.GetFactoryClasses().Rows.OfType<DataRow>()
                    .Select(x => new DbProviderInfo
                    {
                        Name = x["Name"]?.ToString(),
                        Description = x["Description"]?.ToString(),
                        InvariantName = x["InvariantName"]?.ToString(),
                    })
                    .OrderBy(x => x.Name)
                    .Select(x => new DbProvider(x))
                    .ForEach(x => providers[x.ProviderInvariantName] = x);

                IsProvidersInitialized = true;
            }

            return providers;
        }

        public virtual DbConnection CreateConnection()
        {
            var connection = DbProviderFactory.CreateConnection();
            if (_connectionType == null)
            {
                _connectionType = connection?.GetType();
            }

            return connection;
        }

        public virtual DbConnectionString CreateConnectionString(string connectionString = null)
        {
            var connectionStringBuilder = DbProviderFactory.CreateConnectionStringBuilder();
            if (connectionStringBuilder == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                connectionStringBuilder.ConnectionString = connectionString;
            }

            return new DbConnectionString(connectionStringBuilder, Info);
        }

        protected virtual DbProviderInfo GetInfo()
        {
            if (_info != null)
            {
                return _info;
            }

            var infoRow = DbProviderFactories.GetFactoryClasses()
                .Rows.OfType<DataRow>()
                .FirstOrDefault(x => x["InvariantName"]?.ToString() == ProviderInvariantName);

            if (infoRow == null)
            {
                return null;
            }

            _info = new DbProviderInfo
            {
                Name = infoRow["Name"]?.ToString(),
                Description = infoRow["Description"]?.ToString(),
                InvariantName = infoRow["InvariantName"]?.ToString(),
            };

            return _info;
        }
        #endregion
    }
}
