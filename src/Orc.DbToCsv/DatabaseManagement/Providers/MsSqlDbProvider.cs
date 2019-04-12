// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsSqlDbProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    public class MsSqlDbProvider : DbProviderBase
    {
        public MsSqlDbProvider()
            : base("System.Data.SqlClient")
        {
        }
    }
}
