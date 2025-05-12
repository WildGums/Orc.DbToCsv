namespace Orc.DbToCsv.Tests
{
    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class BackupConfigurationTests
    {
        [Test]
        public void Project_Parse_WithBackupConfiguration_LoadsCorrectly()
        {
            // Arrange
            var xaml = @"
<Project xmlns='http://schemas.wildgums.com/orc/dbtocsv'>
  <ConnectionString>Data Source=.\SQLExpress;Initial Catalog=TestDB;Integrated Security=True</ConnectionString>
  <MaximumRowsInTable>1000</MaximumRowsInTable>
  <BackupFileCount>3</BackupFileCount>
  <BackupLocation>Backups</BackupLocation>
  <BackupFormat>{filename}_backup_{timestamp}{extension}</BackupFormat>
  <Project.Tables>
    <Table Name='TestTable' Csv='Test.csv'/>
  </Project.Tables>
</Project>".Replace('\'', '\"');

            // Act
            var project = Project.Parse(xaml);

            // Assert
            Assert.That(project, Is.Not.Null);
            Assert.That(project.BackupFileCount.Value, Is.EqualTo(3));
            Assert.That(project.BackupLocation.Value, Is.EqualTo("Backups"));
            Assert.That(project.BackupFormat.Value, Is.EqualTo("{filename}_backup_{timestamp}{extension}"));
        }

        [Test]
        public void Project_Parse_WithoutBackupConfiguration_HasDefaultValues()
        {
            // Arrange
            var xaml = @"
<Project xmlns='http://schemas.wildgums.com/orc/dbtocsv'>
  <ConnectionString>Data Source=.\SQLExpress;Initial Catalog=TestDB;Integrated Security=True</ConnectionString>
  <MaximumRowsInTable>1000</MaximumRowsInTable>
  <Project.Tables>
    <Table Name='TestTable' Csv='Test.csv'/>
  </Project.Tables>
</Project>".Replace('\'', '\"');

            // Act
            var project = Project.Parse(xaml);

            // Assert
            Assert.That(project, Is.Not.Null);
            Assert.That(project.BackupFileCount.Value, Is.EqualTo(0), "Default BackupFileCount should be 0 (delete all backups)");
            Assert.That(project.BackupLocation.Value, Is.Null.Or.Empty, "Default BackupLocation should be empty");
            Assert.That(project.BackupFormat.Value, Is.EqualTo("{filename}_backup_{timestamp}{extension}"), 
                "Default BackupFormat should be '{filename}_backup_{timestamp}{extension}'");
        }
    }
}