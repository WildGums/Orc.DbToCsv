// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqLiteSourceGateway.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using DataAccess;

    [ConnectToProvider("System.Data.SQLite")]
    public class SqLiteSourceGateway : SqlDbSourceGatewayBase
    {
        #region Constructors
        public SqLiteSourceGateway(DatabaseSource source)
            : base(source)
        {
        }
        #endregion

        #region Methods
        public override DataSourceParameters GetQueryParameters()
        {
            return new DataSourceParameters();
        }

        public override IList<DbObject> GetObjects()
        {
            var tableType = Source.TableType;
            var tableTypeStr = string.Empty;
            switch (tableType)
            {
                case TableType.Function:
                case TableType.Sql:
                case TableType.StoredProcedure:
                    return new List<DbObject>();

                case TableType.Table:
                    tableTypeStr = "table";
                    break;

                case TableType.View:
                    tableTypeStr = "view";
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            var query = $"SELECT name FROM sqlite_master WHERE type ='{tableTypeStr}' AND name NOT LIKE 'sqlite_%';";
            var dbObjects = ReadAllDbObjects(x => x.GetReaderSql(query));

            return dbObjects;
        }
        #endregion
    }
}
