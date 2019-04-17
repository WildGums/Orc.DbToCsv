// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsSqlDbSourceGateway.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Collections.Generic;
    using SqlKata;

    [ConnectToProvider("System.Data.SqlClient")]
    public class MsSqlDbSourceGateway : SqlDbSourceGatewayBase
    {
        #region Constructors
        public MsSqlDbSourceGateway(DatabaseSource source)
            : base(source)
        {
        }
        #endregion

        #region Methods
        public override DbQueryParameters GetQueryParameters()
        {
            var source = Source;
            switch (source.TableType)
            {
                case TableType.Sql:
                    return new DbQueryParameters();

                case TableType.StoredProcedure:
                case TableType.Function:
                {
                    var query = $"SELECT [name], type_name(user_type_id) as type FROM [sys].[parameters] WHERE [object_id] = object_id('{source.Table}')";

                    var connection = GetOpenedConnection();
                    var queryParameters = new DbQueryParameters();
                    using (var reader = connection.GetReader(query))
                    {
                        while (reader.Read())
                        {
                            var args = new DbQueryParameter
                            {
                                Name = reader.GetString(0),
                                Type = reader.GetString(1)
                            };

                            queryParameters.Parameters.Add(args);
                        }
                    }

                    return queryParameters;
                }

                case TableType.Table:
                    break;

                case TableType.View:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new DbQueryParameters();
        }

        public override IList<DbObject> GetObjects()
        {
            string literalType;
            var tableType = Source.TableType;
            switch (tableType)
            {
                case TableType.StoredProcedure:
                    literalType = "P";
                    break;

                case TableType.Function:
                    literalType = "IF";
                    break;

                case TableType.Table:
                    literalType = "U";
                    break;

                case TableType.View:
                    literalType = "V";
                    break;

                case TableType.Sql:
                    return new List<DbObject>();

                default:
                    throw new ArgumentOutOfRangeException(nameof(tableType), tableType, null);
            }
            
            var query = new Query("dbo.sysobjects")
                .Select("name")
                .Where("uid", 1)
                .Where("type", literalType)
                .OrderBy("name");

            var dbObjects = ReadAllDbObjects(x => x.ExecuteReader(query));

            return dbObjects;
        }
        #endregion
    }
}
