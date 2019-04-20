// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OracleCompiler.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    [ConnectToProvider("Oracle.ManagedDataAccess.Client")]
    public class OracleCompiler: SqlCompilerBase
    {
        public OracleCompiler()
            : base(new SqlKata.Compilers.OracleCompiler())
        {
            
        }
    }
}
