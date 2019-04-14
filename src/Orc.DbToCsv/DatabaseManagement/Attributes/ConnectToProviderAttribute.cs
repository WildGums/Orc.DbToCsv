// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectToProviderAttribute.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ConnectToProviderAttribute : Attribute
    {
        #region Constructors
        public ConnectToProviderAttribute(string providerInvariantName)
        {
            ProviderInvariantName = providerInvariantName;
        }
        #endregion

        #region Properties
        public string ProviderInvariantName { get; }
        #endregion
    }
}
