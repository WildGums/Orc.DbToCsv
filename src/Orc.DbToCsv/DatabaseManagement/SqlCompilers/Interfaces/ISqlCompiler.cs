// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlCompiler.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.DbToCsv.DatabaseManagement
{
    using SqlKata;

    public interface ISqlCompiler
    {
        string Compile(Query query);
    }
}
