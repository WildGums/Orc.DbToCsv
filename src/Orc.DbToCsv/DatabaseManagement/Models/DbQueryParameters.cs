// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbQueryParameters.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using Catel.Data;

    [Serializable]
    public class DbQueryParameters : SavableModelBase<DbQueryParameters>
    {
        #region Constructors
        public DbQueryParameters()
        {
            Parameters = new List<DbQueryParameter>();
        }
        #endregion

        #region Properties
        public List<DbQueryParameter> Parameters { get; set; }
        #endregion
    }
}
