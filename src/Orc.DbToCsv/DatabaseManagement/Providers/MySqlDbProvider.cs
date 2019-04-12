// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySqlDbProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    public class MySqlDbProvider : DbProviderBase
    {
        public MySqlDbProvider()
            : base("MySql.Data.MySqlClient")
        {
        }
    }
}
