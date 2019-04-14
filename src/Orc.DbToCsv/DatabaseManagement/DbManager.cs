// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbManager.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    using System.Collections.Generic;
    using Catel;

    public class DbManager : IDbManager
    {
        #region Constructors
        public DbManager(IDbManagerInitializer dbManagerInitializer)
        {
            Argument.IsNotNull(() => dbManagerInitializer);

            _dbManagerInitializer = dbManagerInitializer;
            _dbProviders = new Dictionary<string, DbProvider>();
        }
        #endregion

        #region Properties
        public IReadOnlyDictionary<string, DbProvider> DbProviders => _dbProviders;
        #endregion

        #region Fields
        private readonly IDbManagerInitializer _dbManagerInitializer;
        private readonly Dictionary<string, DbProvider> _dbProviders;
        #endregion

        #region IDbManager Members
        public void Initialize()
        {
            _dbManagerInitializer.Initialize();
        }

        public void AddProvider(DbProvider dbProvider)
        {
            Argument.IsNotNull(() => dbProvider);

            _dbProviders[dbProvider.ProviderInvariantName] = dbProvider;
        }
        #endregion
    }
}
