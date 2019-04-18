// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbProviderFactoryRepository.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using Catel;
    using Catel.Logging;

    public class DbProviderFactoryRepository
    {
        #region Constants
        /// <summary>
        /// Name of the configuration element.
        /// </summary>
        private const string DbProviderFactoriesElement = "DbProviderFactories";
        #endregion

        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private DataTable _dbProviderFactoryTable;
        #endregion

        #region Methods
        /// <summary>
        /// Adds the specified provider.
        /// </summary>
        /// <param name="providerInfo">The provider.</param>
        public void Add(DbProviderInfo providerInfo)
        {
            Argument.IsNotNull(() => providerInfo);

            var providerTable = GetProviderTable();
            if (providerTable == null)
            {
                return;
            }

            Remove(providerInfo);
            providerTable.Rows.Add(providerInfo.Name, providerInfo.Description, providerInfo.InvariantName, providerInfo.AssemblyQualifiedName);
        }

        /// <summary>
        /// Deletes the specified provider if present.
        /// </summary>
        /// <param name="providerInfo">The provider.</param>
        public void Remove(DbProviderInfo providerInfo)
        {
            Argument.IsNotNull(() => providerInfo);

            var providerTable = GetProviderTable();

            var row = providerTable?.Rows.Cast<DataRow>()
                .FirstOrDefault(o => o[2] != null && o[2].ToString() == providerInfo.InvariantName);

            if (row != null)
            {
                providerTable.Rows.Remove(row);
            }
        }

        private DataTable GetProviderTable()
        {
            if (_dbProviderFactoryTable != null)
            {
                return _dbProviderFactoryTable;
            }

            // Open the configuration.
            if (!(ConfigurationManager.GetSection("system.data") is DataSet dataConfiguration))
            {
                Log.Error("Unable to open 'System.Data' from the configuration");

                return null;
            }

            // Open the provider table.
            if (!dataConfiguration.Tables.Contains(DbProviderFactoriesElement))
            {
                Log.Error($"Unable to open the '{DbProviderFactoriesElement}' table");

                return null;
            }

            _dbProviderFactoryTable = dataConfiguration.Tables[DbProviderFactoriesElement];

            return _dbProviderFactoryTable;
        }
        #endregion
    }
}
