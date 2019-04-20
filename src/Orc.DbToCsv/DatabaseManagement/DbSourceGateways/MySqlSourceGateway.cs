// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySqlSourceGateway.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using DataAccess;

    [ConnectToProvider("MySql.Data.MySqlClient")]
    public class MySqlSourceGateway : SqlDbSourceGatewayBase
    {
        #region Constructors
        public MySqlSourceGateway(DatabaseSource source) : base(source)
        {
        }
        #endregion

        #region Properties
        protected override Dictionary<TableType, Func<DbConnection, DbCommand>> GetObjectListCommandsFactory =>
            new Dictionary<TableType, Func<DbConnection, DbCommand>>
            {
                {TableType.Table, c => c.CreateCommand($"SELECT TABLE_NAME AS NAME FROM information_schema.tables where Table_schema = database() and Table_type = 'BASE TABLE';")},
                {TableType.View, c => c.CreateCommand($"SELECT TABLE_NAME AS NAME FROM information_schema.tables where Table_schema = database() and Table_type = 'VIEW';")},
                {TableType.StoredProcedure, c => c.CreateCommand($"SELECT SPECIFIC_NAME AS NAME FROM INFORMATION_SCHEMA.ROUTINES where ROUTINE_SCHEMA = database() and ROUTINE_TYPE = 'PROCEDURE';")},
            };
        #endregion

        #region Methods
        public override DataSourceParameters GetQueryParameters()
        {
            var source = Source;
            switch (source.TableType)
            {
                case TableType.Table:
                    break;

                case TableType.View:
                    break;

                case TableType.Sql:
                    return new DataSourceParameters();

                case TableType.StoredProcedure:
                case TableType.Function:
                {
                    var query = $"SELECT PARAMETER_NAME AS NAME, DATA_TYPE AS TYPE FROM information_schema.parameters WHERE SPECIFIC_NAME = '{source.Table}' and PARAMETER_MODE = 'IN';";
                    return ReadParametersFromQuery(query);
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new DataSourceParameters();
        }

        protected override DbCommand CreateFunctionCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount)
        {
            throw new NotSupportedException("Table valued function in MySql not supported");
        }
        #endregion
    }
}
