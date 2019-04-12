// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv
{
    using System;
    using System.Data.Common;
    using Catel;

    public static class StringExtensions
    {
        //public static string EncryptConnectionString(this string connectionString, string providerName)
        //{
        //    return AlterConnectionStringPropertyValue(connectionString, providerName, x => x.Encrypt());
        //}

        //public static string DecryptConnectionString(this string connectionString, string providerName)
        //{
        //    return AlterConnectionStringPropertyValue(connectionString, providerName, x => x.Decrypt());
        //}

        //private static string AlterConnectionStringPropertyValue(this string connectionString, string providerName, Func<string, string> alteractionFunction)
        //{
        //    Argument.IsNotNullOrEmpty(() => connectionString);
        //    Argument.IsNotNullOrEmpty(() => providerName);
        //    Argument.IsNotNull(() => alteractionFunction);

        //    var factory = DbProviderFactories.GetFactory(providerName);
        //    var connectionStringBuilder = factory.CreateConnectionStringBuilder();
        //    if (connectionStringBuilder == null)
        //    {
        //        return connectionString;
        //    }

        //    connectionStringBuilder.ConnectionString = connectionString;
        //    var sqlConnectionString = new SqlConnectionString(connectionStringBuilder, new DbProvider());

        //    var sensitiveProperties = sqlConnectionString.Properties.Where(x => x.Value.IsSensitive);
        //    foreach (var sensitiveProperty in sensitiveProperties)
        //    {
        //        var value = connectionStringBuilder[sensitiveProperty.Key].ToString();
        //        if (!string.IsNullOrWhiteSpace(value))
        //        {
        //            connectionStringBuilder[sensitiveProperty.Key] = alteractionFunction(value);
        //        }
        //    }

        //    return connectionStringBuilder.ConnectionString;
        //}

        //public static string GetConnectionStringProperty(this string connectionString, string providerName, string propertyName)
        //{
        //    Argument.IsNotNullOrEmpty(() => connectionString);
        //    Argument.IsNotNullOrEmpty(() => providerName);

        //    var factory = DbProviderFactories.GetFactory(providerName);
        //    var connectionStringBuilder = factory.CreateConnectionStringBuilder();
        //    if (connectionStringBuilder == null)
        //    {
        //        return connectionString;
        //    }

        //    connectionStringBuilder.ConnectionString = connectionString;
        //    var sqlConnectionString = new SqlConnectionString(connectionStringBuilder, new DbProvider());
        //    if (sqlConnectionString.Properties.TryGetValue(propertyName, out var dataSourceProperty))
        //    {
        //        return dataSourceProperty.Value?.ToString() ?? string.Empty;
        //    }

        //    return null;
        //}
    }
}
