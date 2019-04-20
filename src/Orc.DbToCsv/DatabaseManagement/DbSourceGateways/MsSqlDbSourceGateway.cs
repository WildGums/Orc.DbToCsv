// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsSqlDbSourceGateway.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using DataAccess;

    [ConnectToProvider("System.Data.SqlClient")]
    public class MsSqlDbSourceGateway : SqlDbSourceGatewayBase
    {
        #region Constructors
        public MsSqlDbSourceGateway(DatabaseSource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        protected override Dictionary<TableType, Func<DbConnection, DbCommand>> GetObjectListCommandsFactory =>
            new Dictionary<TableType, Func<DbConnection, DbCommand>>
            {
                {TableType.Table, c => CreateGetObjectsCommand(c, "U")},
                {TableType.View, c => CreateGetObjectsCommand(c, "V")},
                {TableType.StoredProcedure, c => CreateGetObjectsCommand(c, "P")},
                {TableType.Function, c => CreateGetObjectsCommand(c, "IF")},
            };
        #endregion

        #region Methods
        public override DataSourceParameters GetQueryParameters()
        {
            var source = Source;
            switch (source.TableType)
            {
                case TableType.Sql:
                    return new DataSourceParameters();

                case TableType.StoredProcedure:
                case TableType.Function:
                {
                    var query = $"SELECT [name], type_name(user_type_id) as type FROM [sys].[parameters] WHERE [object_id] = object_id('{source.Table}')";
                    return ReadParametersFromQuery(query);
                }

                case TableType.Table:
                    break;

                case TableType.View:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new DataSourceParameters();
        }

        private DbCommand CreateGetObjectsCommand(DbConnection connection, string commandParameter)
        {
            return connection.CreateCommand($"SELECT name FROM dbo.sysobjects WHERE uid = 1 AND type = '{commandParameter}' ORDER BY name;");
        }
        #endregion
    }
}
