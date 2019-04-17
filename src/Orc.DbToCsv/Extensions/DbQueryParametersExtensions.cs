// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbQueryParametersExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Collections.Generic;
    using System.Linq;
    using DatabaseManagement;

    public static class DbQueryParametersExtensions
    {
        public static string ToArgsValueString(this DbQueryParameters queryParameters)
        {
            return queryParameters != null ? string.Join(",", queryParameters.Parameters?.Select(x => $"'{x.Value}'") ?? new List<string>()) : string.Empty;
        }

        public static string ToArgsNamesString(this DbQueryParameters queryParameters, string argsPrefix = "")
        {
            return queryParameters != null ? string.Join(",", queryParameters.Parameters?.Select(x => $"{argsPrefix}{x.Name}") ?? new List<string>()) : string.Empty;
        }
    }
}
