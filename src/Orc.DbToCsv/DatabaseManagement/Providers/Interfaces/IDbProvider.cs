// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDbProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    using System.Data.Common;

    public interface IDbProvider
    {
        #region Properties
        string ProviderInvariantName { get; }
        #endregion

        #region Methods
        DbConnection CreateConnection(DatabaseSource source);
        #endregion
    }
}
