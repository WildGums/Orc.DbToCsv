// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirebirdSourceGateway.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using DataAccess;

    [ConnectToProvider("FirebirdSql.Data.FirebirdClient")]
    public class FirebirdSourceGateway : SqlDbSourceGatewayBase
    {
        #region Constructors
        public FirebirdSourceGateway(DatabaseSource source)
            : base(source)
        {
        }
        #endregion

        #region Properties
        protected override Dictionary<TableType, Func<DbConnection, DbCommand>> GetObjectListCommandsFactory =>
            new Dictionary<TableType, Func<DbConnection, DbCommand>>
            {
                {TableType.Table, c => c.CreateCommand($"SELECT rdb$relation_name FROM rdb$relations WHERE rdb$view_blr is null AND (rdb$system_flag is null OR rdb$system_flag = 0);")},
                {TableType.View, c => c.CreateCommand($"SELECT rdb$relation_name FROM rdb$relations WHERE rdb$view_blr is not null AND (rdb$system_flag is null OR rdb$system_flag = 0);")},
                {TableType.StoredProcedure, c => c.CreateCommand("SELECT rdb$procedure_name FROM rdb$procedures;")},
                {TableType.Function, c => c.CreateCommand($"SELECT rdb$function_name FROM rdb$functions;")},
            };

        protected override Dictionary<TableType, Func<DataSourceParameters>> DataSourceParametersFactory => new Dictionary<TableType, Func<DataSourceParameters>>()
        {
            {TableType.StoredProcedure, () => GetArgs($"SELECT rdb$parameter_name, rdb$parameter_type FROM rdb$procedure_parameters WHERE rdb$procedure_name = '{Source.Table}'")},
            {TableType.Function, () => GetArgs($"SELECT rdb$argument_name, rdb$field_type FROM rdb$procedure_parameters WHERE rdb$procedure_name = '{Source.Table}'")},
        };
        #endregion

        #region Methods
        protected override DbCommand CreateGetTableRecordsCommand(DbConnection connection, DataSourceParameters parameters, int offset, int fetchCount, bool isPagingEnabled)
        {
            var source = Source;
            var query = isPagingEnabled
                ? offset == 0
                    ? $"SELECT FIRST {fetchCount} * FROM \"{source.Table}\""
                    : $"SELECT * FROM \"{source.Table}\" ROWS {offset + 1} TO {offset + fetchCount}"
                : $"SELECT * FROM \"{source.Table}\"";

            return connection.CreateCommand(query);
        }

        protected override DbCommand CreateTableCountCommand(DbConnection connection)
        {
            return connection.CreateCommand($"SELECT COUNT(*) AS \"COUNT\" FROM \"{Source.Table}\"");
        }
        #endregion
    }
}
