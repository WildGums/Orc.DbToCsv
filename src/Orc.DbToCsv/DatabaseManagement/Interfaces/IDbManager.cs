﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDbManager.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    using System.Collections.Generic;

    public interface IDbManager
    {
        #region Properties
        IReadOnlyDictionary<string, DbProvider> DbProviders { get; }
        #endregion

        #region Methods
        void Initialize();

        void AddProvider(DbProvider dbProvider);
        #endregion
    }
}