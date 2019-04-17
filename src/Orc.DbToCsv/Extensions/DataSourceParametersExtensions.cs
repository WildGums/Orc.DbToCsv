// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSourceParametersExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;

    public static class DataSourceParametersExtensions
    {
        #region Methods
        public static string ToArgsValueString(this DataSourceParameters queryParameters)
        {
            return queryParameters != null ? string.Join(",", queryParameters.Parameters?.Select(x => $"'{x.Value}'") ?? new List<string>()) : string.Empty;
        }

        public static string ToArgsNamesString(this DataSourceParameters queryParameters, string argsPrefix = "")
        {
            return queryParameters != null ? string.Join(",", queryParameters.Parameters?.Select(x => $"{argsPrefix}{x.Name}") ?? new List<string>()) : string.Empty;
        }
        #endregion
    }
}
