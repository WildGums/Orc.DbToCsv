// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirebirdCompiler.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    [ConnectToProvider("FirebirdSql.Data.FirebirdClient")]
    public class FirebirdCompiler : SqlCompilerBase
    {
        #region Constructors
        public FirebirdCompiler()
            : base(new SqlKata.Compilers.FirebirdCompiler())
        {
        }
        #endregion
    }
}
