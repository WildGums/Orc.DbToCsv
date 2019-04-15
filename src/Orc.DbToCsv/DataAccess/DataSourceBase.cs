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
    using System.Linq;
    using System.Reflection;
    using Catel.Caching;
    using Catel.Data;
    using Catel.Reflection;
    using ValidationContext = Catel.Data.ValidationContext;

    public abstract class DataSourceBase : ModelBase
    {
        #region Constants
        private static readonly CacheStorage<Type, PropertyInfo[]> PropertiesCache = new CacheStorage<Type, PropertyInfo[]>();
        #endregion

        #region Fields
        protected readonly Dictionary<string, string> Properties = new Dictionary<string, string>();
        #endregion

        #region Constructors
        protected DataSourceBase()
            : this(string.Empty)
        {
        }

        protected DataSourceBase(string location)
        {
            ParseLocation(location);
        }
        #endregion

        #region Properties
        public IValidationContext ValidationContext { get; private set; }
        #endregion

        #region Methods
        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs args)
        {
            var propertyName = args.PropertyName;
            if (propertyName == nameof(IsDirty) || propertyName == nameof(ValidationContext))
            {
                return;
            }

            base.OnPropertyChanged(args);

            if (args.NewValue == null)
            {
                Properties.Remove(propertyName);
            }
            else
            {
                Properties[propertyName] = args.NewValue?.ToString();
            }

            Validate();
        }

        public IReadOnlyDictionary<string, string> AsDictionary()
        {
            return Properties;
        }

        public void SetProperty(string propertyName, string propertyValueStr)
        {
            var type = GetType();
            var properties = PropertiesCache.GetFromCacheOrFetch(type, () => type.GetPropertiesEx());
            var property = properties.FirstOrDefault(x => x.Name == propertyName);
            SetPropertyValue(property, propertyValueStr);
        }

        public virtual void Validate()
        {
            using (SuspendChangeNotifications(false))
            {
                ValidationContext = new ValidationContext();

                var type = GetType();
                var properties = PropertiesCache.GetFromCacheOrFetch(type, () => type.GetPropertiesEx());
                foreach (var property in properties)
                {
                    if (!property.IsDecoratedWithAttribute(typeof(RequiredAttribute)))
                    {
                        continue;
                    }

                    var propertyValue = property.GetValue(this);
                    if (propertyValue == null)
                    {
                        ValidationContext.AddValidationError($"Required field '{property.Name}' is empty");
                    }
                }
            }
        }

        private void ParseLocation(string location)
        {
            var type = GetType();
            var properties = PropertiesCache.GetFromCacheOrFetch(type, () => type.GetPropertiesEx());
            var dictionary = KeyValueStringParser.Parse(location);
            using (SuspendChangeNotifications(false))
            {
                ValidationContext = new ValidationContext();

                foreach (var property in properties)
                {
                    var propertyName = property.Name;

                    if (dictionary.TryGetValue(propertyName, out var propertyValueStr))
                    {
                        dictionary.Remove(propertyName);

                        SetPropertyValue(property, propertyValueStr);
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
        }

        private void SetPropertyValue(PropertyInfo property, string propertyValueStr)
        {
            var propertyName = property.Name;
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
        #endregion
    }
}
