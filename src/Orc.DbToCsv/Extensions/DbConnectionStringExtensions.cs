// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbConnectionStringExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using Catel;
    using DatabaseManagement;

    public static class DbConnectionStringExtensions
    {
        #region Methods
        public static DbConnectionStringProperty TryGetProperty(this DbConnectionString connectionString, string propertyName)
        {
            Argument.IsNotNull(() => connectionString);

            var properties = connectionString?.Properties;
            if (properties == null)
            {
                return null;
            }

            var upperInvariantPropertyName = propertyName.ToUpperInvariant();
            if (properties.TryGetValue(upperInvariantPropertyName, out var property))
            {
                return property;
            }

            if (properties.TryGetValue(upperInvariantPropertyName.Replace(" ", string.Empty), out property))
            {
                return property;
            }

            return null;
        }
        #endregion
    }
}
