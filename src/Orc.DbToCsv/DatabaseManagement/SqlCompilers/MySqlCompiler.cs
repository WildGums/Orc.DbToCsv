// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySqlCompiler.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    [ConnectToProvider("MySql.Data.MySqlClient")]
    public class MySqlCompiler : SqlCompilerBase
    {
        public MySqlCompiler()
            : base(new SqlKata.Compilers.MySqlCompiler())
        {
        }
    }
}
