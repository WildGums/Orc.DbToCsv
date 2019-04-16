// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbObject.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    public class DbObject
    {
        #region Constructors
        public DbObject(TableType type)
        {
            Type = type;
        }
        #endregion

        #region Properties
        public string Name { get; set; }
        public TableType Type { get; }
        #endregion
    }
}
