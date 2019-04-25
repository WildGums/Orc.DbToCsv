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
        #region Fields
        private readonly DbConnectionStringBuilder _connectionStringBuilder;
        #endregion

        #region Constructors
        public DbConnectionString(DbConnectionStringBuilder connectionStringBuilder, DbProviderInfo dbProvider)
        {
            Argument.IsNotNull(() => connectionStringBuilder);
            Argument.IsNotNull(() => dbProvider);

            _connectionStringBuilder = connectionStringBuilder;
            DbProvider = dbProvider;

            UpdateProperties();
        }
        #endregion

        #region Properties
        public IReadOnlyDictionary<string, DbConnectionStringProperty> Properties { get; private set; }

        public DbProviderInfo DbProvider { get; }

        public DbConnectionStringBuilder ConnectionStringBuilder => _connectionStringBuilder;
        #endregion

        #region Methods
        private void UpdateProperties()
        {
            if (_connectionStringBuilder == null)
            {
                Properties = null;
                return;
            }

            var sensitiveProperties = TypeDescriptor.GetProperties(_connectionStringBuilder, new Attribute[] {PasswordPropertyTextAttribute.Yes})
                .OfType<PropertyDescriptor>()
                .Select(x => x.DisplayName.ToUpperInvariant());

            var sensitivePropertiesHashSet = new HashSet<string>();
            sensitivePropertiesHashSet.AddRange(sensitiveProperties);

            
            var propDescriptor = _connectionStringBuilder as ICustomTypeDescriptor;
            var props = propDescriptor.GetProperties().OfType<PropertyDescriptor>().Where(x => x.GetType().Name =="DbConnectionStringBuilderDescriptor").ToList();

            Properties = props
                .ToDictionary(x => x.DisplayName.ToUpperInvariant(), x =>
                {
                    var isSensitive = sensitivePropertiesHashSet.Contains(x.DisplayName.ToUpperInvariant());
                    return new DbConnectionStringProperty(isSensitive, _connectionStringBuilder, x);
                });
        }

        public virtual string ToDisplayString()
        {
            var sensitiveProperties = Properties.Values.Where(x => x.IsSensitive);

            var removedProperties = new List<Tuple<string, object>>();
            foreach (var sensitiveProperty in sensitiveProperties)
            {
                var propertyName = sensitiveProperty.Name;
                if (!_connectionStringBuilder.ShouldSerialize(propertyName))
                {
                    continue;
                }

                removedProperties.Add(new Tuple<string, object>(propertyName, _connectionStringBuilder[propertyName]));
                _connectionStringBuilder.Remove(propertyName);
            }

            var displayConnectionString = _connectionStringBuilder.ConnectionString;

            foreach (var prop in removedProperties.Where(prop => prop.Item2 != null))
            {
                _connectionStringBuilder[prop.Item1] = prop.Item2;
            }

            return displayConnectionString;
        }

        public override string ToString()
        {
            return _connectionStringBuilder.ConnectionString;
        }
        #endregion

    }
}
