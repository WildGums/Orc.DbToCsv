// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbManagerInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    using Catel;
    using Catel.IoC;

    public class DbManagerInitializer : IDbManagerInitializer
    {
        #region Constructors
        public DbManagerInitializer(IServiceLocator serviceLocator, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => serviceLocator);
            Argument.IsNotNull(() => typeFactory);

            _serviceLocator = serviceLocator;
            _typeFactory = typeFactory;
        }
        #endregion

        #region IDbManagerInitializer Members
        public virtual void Initialize()
        {
            var dbManager = _serviceLocator.ResolveType<IDbManager>();

            //var msSqlProvider = _typeFactory.CreateInstance<MsSqlDbProvider>();
            //var mySqlProvider = _typeFactory.CreateInstance<MySqlDbProvider>();
            //var sqliteProvider = _typeFactory.CreateInstance<SQLiteProvider>();
            //var postgreSqlDbProvider = _typeFactory.CreateInstance<PostgreSQLDbProvider>();

            //dbManager.AddProvider(msSqlProvider);
            //dbManager.AddProvider(mySqlProvider);
            //dbManager.AddProvider(sqliteProvider);
            //dbManager.AddProvider(postgreSqlDbProvider);
        }
        #endregion

        #region Fields
        private readonly IServiceLocator _serviceLocator;
        private readonly ITypeFactory _typeFactory;
        #endregion
    }
}
