﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Database.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Catel;

    public abstract class DbSourceGateway
    {
        #region Fields
        private DbConnection _connection;
        private DbProvider _provider;
        #endregion

        #region Constructors
        public DbSourceGateway(DatabaseSource source)
        {
            Argument.IsNotNull(() => source);

            Source = source;
        }
        #endregion

        #region Properties
        public DatabaseSource Source { get; }
        public DbProvider Provider => _provider ?? (_provider = Source.GetProvider());
        public DbConnection Connection => _connection ?? (_connection = Provider?.CreateConnection(Source));
        #endregion

        #region Methods
        protected DbConnection GetOpenedConnection()
        {
            var connection = Connection;
            if (connection == null)
            {
                return null;
            }

            if (!connection.State.HasFlag(ConnectionState.Open))
            {
                connection.Open();
            }

            return connection;
        }

        public abstract DbDataReader GetRecords(DbQueryParameters queryParameters = null, int offset = 0, int fetchCount = -1);
        public abstract int GetCount(DbQueryParameters queryParameters = null);
        public abstract DbQueryParameters GetQueryParameters();
        public abstract IList<DbObject> GetObjects();
        #endregion
    }
}
