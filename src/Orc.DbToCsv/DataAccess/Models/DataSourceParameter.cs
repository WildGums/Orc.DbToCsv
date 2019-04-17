// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbQueryParameter.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DataAccess
{
    public class DataSourceParameter
    {
        #region Properties
        public string Name { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
        #endregion
    }
}
