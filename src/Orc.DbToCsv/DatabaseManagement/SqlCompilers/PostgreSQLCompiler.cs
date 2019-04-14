// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSQLCompiler.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    using SqlKata.Compilers;

    [ConnectToProvider("Npgsql")]
    public class PostgreSqlCompiler : SqlCompilerBase
    {
        public PostgreSqlCompiler()
            : base(new PostgresCompiler())
        {
        }
    }
}
