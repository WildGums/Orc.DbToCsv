// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsSqlCompiler.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    using SqlKata.Compilers;

    [ConnectToProvider("System.Data.SqlClient")]
    public class MsSqlCompiler : SqlCompilerBase
    {
        public MsSqlCompiler()
            : base(new SqlServerCompiler())
        {
            
        }
    }
}
