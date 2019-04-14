// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlCompilerBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using Catel;
    using SqlKata;
    using SqlKata.Compilers;

    public abstract class SqlCompilerBase : ISqlCompiler
    {
        #region Fields
        private readonly Compiler _compiler;
        #endregion

        #region Constructors
        protected SqlCompilerBase(Compiler compiler)
        {
            Argument.IsNotNull(() => compiler);

            _compiler = compiler;
        }
        #endregion

        #region ISqlCompiler Members
        public string Compile(Query query)
        {
            var result = _compiler.Compile(query);
            return result.ToString();
        }
        #endregion
    }
}
