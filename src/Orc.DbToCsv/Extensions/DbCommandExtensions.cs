// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbCommandExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Data.Common;
    using Catel;
    using DataAccess;

    public static class DbCommandExtensions
    {
        #region Methods
        public static DbCommand AddParameter(this DbCommand dbCommand, DataSourceParameter parameter)
        {
            Argument.IsNotNull(() => dbCommand);
            Argument.IsNotNull(() => parameter);

            return dbCommand.AddParameter(parameter.Name, parameter.Value);
        }

        public static DbCommand AddParameter(this DbCommand dbCommand, string name, object value)
        {
            Argument.IsNotNull(() => dbCommand);

            var parameter = dbCommand.CreateParameter();
            parameter.Value = value;
            parameter.ParameterName = name;
            dbCommand.Parameters.Add(parameter);

            return dbCommand;
        }
        public static long GetRecordsCount(this DbCommand command)
        {
            Argument.IsNotNull(() => command);

            long count = 0;
            using (var reader = command.ExecuteReader())
            {
                while (true)
                {
                    while (reader.Read())
                    {
                        count++;
                    }

                    if (!reader.NextResult())
                    {
                        break;
                    }

                    if (!reader.HasRows)
                    {
                        break;
                    }
                }
            }

            return count;
        }

        #endregion
    }
}
