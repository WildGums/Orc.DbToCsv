﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbToCsvExportDescription.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Collections.Generic;
    using DatabaseManagement;

    public class DbToCsvExportDescription
    {
        public DatabaseSource Source { get; set; }
        public List<Parameter> Parameters { get; set; }
        public string CsvFilePath { get; set; }
    }
}
