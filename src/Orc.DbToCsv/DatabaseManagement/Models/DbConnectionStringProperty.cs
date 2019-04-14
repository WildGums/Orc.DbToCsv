// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbConnectionStringProperty.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System.Data.Common;
    using Catel;
    using Catel.Data;

    public class DbConnectionStringProperty : ObservableObject
    {
        #region Fields
        private readonly DbConnectionStringBuilder _dbConnectionStringBuilder;
        #endregion

        #region Constructors
        public DbConnectionStringProperty(string name, bool isSensitive, DbConnectionStringBuilder dbConnectionStringBuilder)
        {
            Argument.IsNotNull(() => dbConnectionStringBuilder);

            _dbConnectionStringBuilder = dbConnectionStringBuilder;
            Name = name;
            IsSensitive = isSensitive;
        }
        #endregion

        #region Properties
        public string Name { get; }
        public bool IsSensitive { get; }

        public object Value
        {
            get => _dbConnectionStringBuilder[Name];

            set
            {
                var name = Name;
                if (_dbConnectionStringBuilder[name] == value)
                {
                    return;
                }

                _dbConnectionStringBuilder[name] = value;
                RaisePropertyChanged(nameof(Value));
            }
        }
        #endregion
    }

}
