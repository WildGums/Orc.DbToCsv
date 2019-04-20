// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirebirdSourceGateway.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System.Collections.Generic;
    using DataAccess;

    [ConnectToProvider("FirebirdSql.Data.FirebirdClient")]
    public class FirebirdSourceGateway : SqlDbSourceGatewayBase
    {
        #region Constructors
        public FirebirdSourceGateway(DatabaseSource source) : base(source)
        {
        }
        #endregion

        #region Methods
        public override DataSourceParameters GetQueryParameters()
        {
            return new DataSourceParameters();
        }

        public override IList<DbObject> GetObjects()
        {
            return new List<DbObject>();
        }
        #endregion
    }
}
