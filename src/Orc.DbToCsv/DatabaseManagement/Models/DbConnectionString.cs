// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbConnectionString.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Common;
    using System.Linq;
    using Catel;
    using Catel.Collections;
    using Catel.Data;

    public class DbConnectionString : ModelBase
    {
        #region Constructors
        public DbConnectionString(DbConnectionStringBuilder connectionStringBuilder, DbProviderInfo dbProvider)
        {
            Argument.IsNotNull(() => connectionStringBuilder);
            Argument.IsNotNull(() => dbProvider);

            ConnectionStringBuilder = connectionStringBuilder;
            DbProvider = dbProvider;

            UpdateProperties();
        }
        #endregion

        #region Properties
        public DbConnectionStringBuilder ConnectionStringBuilder { get; }

        public IReadOnlyDictionary<string, DbConnectionStringProperty> Properties { get; private set; }

        public DbProviderInfo DbProvider { get; }
        #endregion

        #region Methods
        private void UpdateProperties()
        {
            var connectionStringBuilder = ConnectionStringBuilder;
            var sensitiveProperties = TypeDescriptor.GetProperties(connectionStringBuilder, new Attribute[] {PasswordPropertyTextAttribute.Yes})
                .OfType<PropertyDescriptor>()
                .Select(x => x.DisplayName);

            var sensitivePropertiesHashSet = new HashSet<string>();
            sensitivePropertiesHashSet.AddRange(sensitiveProperties);

            Properties = connectionStringBuilder.Keys.OfType<string>()
                .ToDictionary(x => x, x =>
                {
                    var isSensitive = sensitivePropertiesHashSet.Contains(x);
                    return new DbConnectionStringProperty(x, isSensitive, connectionStringBuilder);
                });
        }

        public virtual string ToDisplayString()
        {
            var connectionStringBuilder = ConnectionStringBuilder;
            var sensitiveProperties = Properties.Values.Where(x => x.IsSensitive);

            var removedProperties = new List<Tuple<string, object>>();
            foreach (var sensitiveProperty in sensitiveProperties)
            {
                var propertyName = sensitiveProperty.Name;
                if (!connectionStringBuilder.ShouldSerialize(propertyName))
                {
                    continue;
                }

                removedProperties.Add(new Tuple<string, object>(propertyName, connectionStringBuilder[propertyName]));
                connectionStringBuilder.Remove(propertyName);
            }

            var displayConnectionString = connectionStringBuilder.ConnectionString;

            foreach (var (key, value) in removedProperties.Where(prop => prop.Item2 != null))
            {
                connectionStringBuilder[key] = value;
            }

            return displayConnectionString;
        }

        public override string ToString()
        {
            return ConnectionStringBuilder.ConnectionString;
        }
        #endregion
    }
}
