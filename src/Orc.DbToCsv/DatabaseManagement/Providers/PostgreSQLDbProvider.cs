// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSQLDbProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    public class PostgreSQLDbProvider : DbProviderBase
    {
        public PostgreSQLDbProvider()
            : base("Npgsql")
        {
        }
    }
}
