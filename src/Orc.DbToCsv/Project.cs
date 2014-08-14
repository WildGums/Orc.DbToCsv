namespace Orc.DbToCsv
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    public class Project
    {
        public string ConnectionString { get; set; }

        public string OutputFolder { get; set; }

        public int MaximumRowsInTable { get; set; } 

        public List<string> Tables { get; set; }

        public Project()
        {
            this.Tables = new List<string>();
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="makeRelativePaths">
        /// The make relative paths.
        /// </param>
        /// <param name="removeNotNeededData">
        /// The remove not needed data.
        /// </param>
        public void Save(string filePath)
        {
            XmlSerializer xmlSerializer = GetSerializer();

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                xmlSerializer.Serialize(sw, this);
            }
        }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static Project Load(string filePath)
        {
            XmlSerializer xmlSerializer = GetSerializer();

            using (StreamReader sr = new StreamReader(filePath))
            {
                Project project = (Project)xmlSerializer.Deserialize(sr);

                return project;
            }
        }

        private static XmlSerializer GetSerializer()
        {
            return new XmlSerializer(
                typeof(Project),
                new[] { typeof(List<string>) });
        }
    }
}
