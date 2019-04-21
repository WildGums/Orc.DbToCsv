// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbSourceGatewayBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Catel;
    using DataAccess;

    public abstract class DbSourceGatewayBase : IDisposable
    {
        #region Fields
        private DbConnection _connection;
        private DbProvider _provider;

        private List<DbConnection> _openedConnections = new List<DbConnection>();
        #endregion

        #region Constructors
        public DbSourceGatewayBase(DatabaseSource source)
        {
            Argument.IsNotNull(() => source);

            Source = source;
        }
        #endregion

        #region Properties
        public DatabaseSource Source { get; }
        public virtual DbProvider Provider => _provider ?? (_provider = Source.GetProvider());
        public virtual DbConnection Connection => _connection ?? (_connection = Provider?.CreateConnection(Source));
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            Close();

            Connection?.Dispose();

            foreach (var openedConnection in _openedConnections)
            {
                openedConnection.Dispose();
            }
        }
        #endregion

        #region Methods
        public abstract DbDataReader GetRecords(DataSourceParameters queryParameters = null, int offset = 0, int fetchCount = -1);
        public abstract long GetCount(DataSourceParameters queryParameters = null);
        public abstract DataSourceParameters GetQueryParameters();
        public abstract IList<DbObject> GetObjects();

        protected DbConnection GetOpenedConnection()
        {
            var connection = Connection;
            if (connection == null)
            {
                return null;
            }

            if (connection.State.HasFlag(ConnectionState.Open))
            {
                var newConnection = Provider?.CreateConnection(Source);
                newConnection?.Open();

                _openedConnections.Add(newConnection);
                return newConnection;
            }

            if (!connection.State.HasFlag(ConnectionState.Open))
            {
                connection.Open();
            }

            return connection;
        }

        public void Close()
        {
            Connection?.Close();

            foreach (var openedConnection in _openedConnections)
            {
                openedConnection.Close();
            }
        }
        #endregion
    }
}
