// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbProviderBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    using System.Data.Common;
    using Catel;

    public abstract class DbProviderBase
    {
        #region Fields
        private DbProviderFactory _dbProviderFactory;
        #endregion

        protected DbProviderBase(string providerInvariantName)
        {
            Argument.IsNotNullOrWhitespace(() => providerInvariantName);

            ProviderInvariantName = providerInvariantName;
        }

        #region Properties
        protected DbProviderFactory DbProviderFactory => _dbProviderFactory ?? (_dbProviderFactory = DbProviderFactories.GetFactory(ProviderInvariantName));
        public string Dialect { get; }
        public string ProviderInvariantName { get; }
        #endregion

        public virtual DbConnection CreateConnection(DatabaseSource source)
        {
            Argument.IsNotNull(() => source);

            var connection = DbProviderFactory.CreateConnection();
            if (connection == null)
            {
                return null;
            }

            connection.ConnectionString = source.ConnectionString;//.DecryptConnectionString(ProviderInvariantName);

            return connection;
        }
    }
}
