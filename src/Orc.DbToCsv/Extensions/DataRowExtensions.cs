// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataRowExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Data;
    using Catel;
    using DatabaseManagement;

    internal static class DataRowExtensions
    {
        #region Methods
        public static DbProviderInfo ToDbProviderInfo(this DataRow row)
        {
            Argument.IsNotNull(() => row);

            return new DbProviderInfo
            {
                Name = row["Name"]?.ToString(),
                Description = row["Description"]?.ToString(),
                InvariantName = row["InvariantName"]?.ToString(),
                AssemblyQualifiedName = row["AssemblyQualifiedName"]?.ToString()
            };
        }
        #endregion
    }
}
