// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Project.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    public class Project
    {
        #region Constructors
        public Project()
        {
            Tables = new List<string>();
        }
        #endregion

        #region Properties
        public string ConnectionString { get; set; }
        public string OutputFolder { get; set; }
        public int MaximumRowsInTable { get; set; }
        public List<string> Tables { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void Save(string filePath)
        {
            var xmlSerializer = GetSerializer();

            using (var sw = new StreamWriter(filePath))
            {
                xmlSerializer.Serialize(sw, this);
            }
        }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The <see cref="object" />.</returns>
        public static Project Load(string filePath)
        {
            var xmlSerializer = GetSerializer();

            using (var sr = new StreamReader(filePath))
            {
                var project = (Project) xmlSerializer.Deserialize(sr);

                return project;
            }
        }

        private static XmlSerializer GetSerializer()
        {
            return new XmlSerializer(typeof (Project),new[] {typeof (List<string>)});
        }
        #endregion
    }
}