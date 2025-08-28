namespace Orc.DbToCsv.Tests
{
    using System.IO;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class CsvImporterTests
    {
        [SetUp]
        public void SetUp()
        {
            // Initialize any test setup
        }

        [Test]
        public void CsvToDbImportDescription_Properties_ShouldBeSettable()
        {
            // Arrange & Act
            var description = new CsvToDbImportDescription
            {
                CsvFilePath = "test.csv",
                TruncateTable = true
            };

            // Assert
            Assert.That(description.CsvFilePath, Is.EqualTo("test.csv"));
            Assert.That(description.TruncateTable, Is.True);
        }

        [Test]
        public void Table_TruncateTable_ShouldDefaultToFalse()
        {
            // Arrange & Act
            var table = new Table();

            // Assert
            Assert.That(table.TruncateTable, Is.False);
        }

        [Test]
        public void Table_TruncateTable_ShouldBeSettable()
        {
            // Arrange
            var table = new Table();

            // Act
            table.TruncateTable = true;

            // Assert
            Assert.That(table.TruncateTable, Is.True);
        }

        [Test]
        public void ProjectExtensions_GetCsvToDbImportDescriptions_ShouldCreateCorrectDescriptions()
        {
            // Arrange
            var project = new Project();
            project.Tables.Add(new Table
            {
                Name = "TestTable",
                Csv = "test.csv",
                Output = "output",
                TruncateTable = true,
                Schema = "dbo",
                Provider = "System.Data.SqlClient"
            });

            project.ConnectionString.Value = "Server=test;Database=test;";
            project.OutputFolder.Value = "C:\\output";

            // Act
            var descriptions = project.GetCsvToDbImportDescriptions();

            // Assert
            Assert.That(descriptions, Is.Not.Null);
            Assert.That(descriptions.Count, Is.EqualTo(1));

            var description = descriptions[0];
            Assert.That(description.TruncateTable, Is.True);
            Assert.That(description.CsvFilePath, Does.Contain("test.csv"));
            Assert.That(description.Target, Is.Not.Null);
            Assert.That(description.Target.Table, Is.EqualTo("TestTable"));
            Assert.That(description.Target.Schema, Is.EqualTo("dbo"));
        }

        [Test]
        public async Task CsvImporter_ProcessProjectAsync_WithNullProject_ShouldNotThrow()
        {
            // Arrange
            string nonExistentPath = "non-existent-file.iprj";

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await CsvImporter.ProcessProjectAsync(nonExistentPath));
        }
    }
}
