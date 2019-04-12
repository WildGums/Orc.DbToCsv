// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SQLiteProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    public class SQLiteProvider : DbProviderBase
    {
        public SQLiteProvider()
            : base("System.Data.SQLite")
        {
        }
    }
}
