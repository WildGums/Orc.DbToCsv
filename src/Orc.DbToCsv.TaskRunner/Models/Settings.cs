// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.TaskRunner.Models
{
    using System.Collections.Generic;
    using Catel.Data;

    public class Settings : ModelBase
    {
        #region Fields
        private string _projectFile;
        #endregion

        #region Constructors
        public Settings()
        {
            ConnectionString = @"Data Source=localhost;Initial Catalog=MyDbBName;Integrated Security=True";
            MaximumRowsInTable = 1000;
            OutputDirectory = "./Output";
            Tables = new List<Table>();
        }
        #endregion

        #region Properties
        public string ProjectFile
        {
            get { return _projectFile; }
            set
            {
                _projectFile = value;
                var project = Project.Load(_projectFile);
                ConnectionString = project.ConnectionString.Value;
                MaximumRowsInTable = project.MaximumRowsInTable.Value;
                OutputDirectory = project.OutputFolder.Value;
                Tables = project.Tables;
            }
        }

        public List<Table> Tables { get; set; }

        public string ConnectionString { get; set; }

        public int MaximumRowsInTable { get; set; }

        public string OutputDirectory { get; set; }
        #endregion
    }
}