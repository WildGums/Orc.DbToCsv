// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseSource.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using DataAccess;

    public class DatabaseSource : DataSourceBase
    {
        #region Constructors
        public DatabaseSource()
            : this(string.Empty)
        {
        }

        public DatabaseSource(string location)
            : base(location)
        {
        }
        #endregion

        #region Properties
        public string Table { get; set; }
        public TableType TableType { get; set; }

        [Required]
        public string ConnectionString { get; set; }

        [Required]
        public string ProviderName { get; set; }
        #endregion

        #region Methods
        protected override bool TryConvertFromString(string propertyName, string propertyValueStr, out object propertyValue)
        {
            if (propertyName != nameof(TableType))
            {
                return base.TryConvertFromString(propertyName, propertyValueStr, out propertyValue);
            }

            if (Enum.TryParse(propertyValueStr, true, out TableType tableType))
            {
                propertyValue = tableType;

                return true;
            }

            propertyValue = null;
            return false;
        }
        #endregion
    }
}
