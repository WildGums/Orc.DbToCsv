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
        public DatabaseSource(string location)
            : base(location)
        {
        }

        #region Properties
        public string Table { get; set; }
        public TableType TableType { get; set; } = TableType.Table;
        [Required]
        public string ConnectionString { get; set; }
        [Required]
        public string ProviderName { get; set; }
        #endregion

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
    }
}
