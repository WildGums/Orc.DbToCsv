// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSourceBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using Catel.Caching;
    using Catel.Data;
    using Catel.Reflection;
    using ValidationContext = Catel.Data.ValidationContext;

    public abstract class DataSourceBase
    {
        private static readonly CacheStorage<Type, PropertyInfo[]> PropertiesCache = new CacheStorage<Type, PropertyInfo[]>();

        protected readonly Dictionary<string, string> Properties = new Dictionary<string, string>();

        protected DataSourceBase()
            : this(string.Empty)
        {
        }

        protected DataSourceBase(string location)
        {
            ParseLocation(location);
        }

        public IValidationContext ValidationContext { get; private set; }

        public IReadOnlyDictionary<string, string> AsDictionary()
        {
            return Properties;
        }

        private void ParseLocation(string location)
        {
            ValidationContext = new ValidationContext();

            var type = GetType();
            var properties = PropertiesCache.GetFromCacheOrFetch(type, () => type.GetPropertiesEx());
            var dictionary = KeyValueStringParser.Parse(location);
            foreach (var property in properties)
            {
                var propertyName = property.Name;

                if (dictionary.TryGetValue(propertyName, out var propertyValueStr))
                {
                    dictionary.Remove(propertyName);

                    if (TryConvertFromString(propertyName, propertyValueStr, out var propertyValue))
                    {
                        property.SetValue(this, propertyValue);
                        Properties[propertyName] = propertyValueStr;
                    }
                    else
                    {
                        ValidationContext.AddValidationError($"Can't convert {propertyValueStr} as {propertyName} value\r\n");
                    }
                }
                else
                {
                    if (property.IsDecoratedWithAttribute(typeof(RequiredAttribute)))
                    {
                        ValidationContext.AddValidationError($"Required field '{propertyName}' is empty");
                    }
                }
            }
        }

        protected virtual bool TryConvertFromString(string propertyName, string propertyValueStr, out object propertyValue)
        {
            propertyValue = propertyValueStr;

            return true;
        }

        public virtual string GetLocation()
        {
            if (ValidationContext.HasErrors)
            {
                return string.Empty;
            }

            return KeyValueStringParser.FormatToKeyValueString(Properties);
        }

        public override string ToString()
        {
            return GetLocation();
        }
    }
}
