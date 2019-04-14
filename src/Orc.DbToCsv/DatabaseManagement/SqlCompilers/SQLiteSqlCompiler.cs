// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SQLiteSqlCompiler.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    using SqlKata.Compilers;

    [ConnectToProvider("System.Data.SQLite")]
    public class SQLiteSqlCompiler : SqlCompilerBase
    {
        public SQLiteSqlCompiler()
            : base(new SqliteCompiler())
        {
        }
    }
}
