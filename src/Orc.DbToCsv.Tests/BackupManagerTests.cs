namespace Orc.DbToCsv.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using NUnit.Framework;

    [TestFixture]
    public class BackupManagerTests
    {
        private string _testDirectory;
        private string _backupDirectory;

        [SetUp]
        public void Setup()
        {
            // Create a unique test directory
            _testDirectory = Path.Combine(Path.GetTempPath(), "DbToCsvTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testDirectory);

            // Create a backup subdirectory
            _backupDirectory = Path.Combine(_testDirectory, "Backups");
            Directory.CreateDirectory(_backupDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test directories
            if (Directory.Exists(_testDirectory))
            {
                try
                {
                    Directory.Delete(_testDirectory, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to clean up test directory: {ex.Message}");
                }
            }
        }

        [Test]
        public void CreateBackup_WithBackupFileCountZero_ShouldDeleteAllBackups()
        {
            // Arrange
            var testFilePath = Path.Combine(_testDirectory, "test.csv");
            File.WriteAllText(testFilePath, "test,data\n1,2\n3,4");

            // Create some existing backups
            for (int i = 1; i <= 3; i++)
            {
                var backupPath = Path.Combine(_testDirectory, $"test_backup_2025050{i}120000.csv");
                File.WriteAllText(backupPath, $"Backup {i}");
            }

            var project = CreateProjectWithBackupSettings(0, string.Empty, "{filename}_backup_{timestamp}{extension}");

            // Act
            // Use reflection to access the internal BackupManager class
            var backupManagerType = typeof(Importer).Assembly.GetType("Orc.DbToCsv.BackupManager");
            var createBackupMethod = backupManagerType.GetMethod("CreateBackup", BindingFlags.Public | BindingFlags.Static);
            
            createBackupMethod.Invoke(null, new object[] { testFilePath, project });

            // Assert
            var backupFiles = Directory.GetFiles(_testDirectory, "test_backup_*.csv");
            Assert.That(backupFiles.Length, Is.EqualTo(0), "All backup files should be deleted when BackupFileCount is 0");
        }

        [Test]
        public void CreateBackup_WithBackupFileCountPositive_ShouldKeepSpecifiedNumberOfBackups()
        {
            // Arrange
            var testFilePath = Path.Combine(_testDirectory, "test.csv");
            File.WriteAllText(testFilePath, "test,data\n1,2\n3,4");

            // Create some existing backups
            for (int i = 1; i <= 5; i++)
            {
                var backupPath = Path.Combine(_testDirectory, $"test_backup_2025050{i}120000.csv");
                File.WriteAllText(backupPath, $"Backup {i}");
                
                // Add a small delay to ensure different timestamps
                System.Threading.Thread.Sleep(100);
            }

            var project = CreateProjectWithBackupSettings(3, string.Empty, "{filename}_backup_{timestamp}{extension}");

            // Act
            // Use reflection to access the internal BackupManager class
            var backupManagerType = typeof(Importer).Assembly.GetType("Orc.DbToCsv.BackupManager");
            var createBackupMethod = backupManagerType.GetMethod("CreateBackup", BindingFlags.Public | BindingFlags.Static);
            
            createBackupMethod.Invoke(null, new object[] { testFilePath, project });

            // Assert
            var backupFiles = Directory.GetFiles(_testDirectory, "test_backup_*.csv");
            Assert.That(backupFiles.Length, Is.EqualTo(3), "Only 3 backup files should be kept when BackupFileCount is 3");
        }

        [Test]
        public void CreateBackup_WithCustomBackupLocation_StoresBackupsInSpecifiedLocation()
        {
            // Arrange
            var testFilePath = Path.Combine(_testDirectory, "test.csv");
            File.WriteAllText(testFilePath, "test,data\n1,2\n3,4");

            var project = CreateProjectWithBackupSettings(1, "Backups", "{filename}_backup_{timestamp}{extension}");

            // Act
            // Use reflection to access the internal BackupManager class
            var backupManagerType = typeof(Importer).Assembly.GetType("Orc.DbToCsv.BackupManager");
            var createBackupMethod = backupManagerType.GetMethod("CreateBackup", BindingFlags.Public | BindingFlags.Static);
            
            createBackupMethod.Invoke(null, new object[] { testFilePath, project });

            // Assert
            var backupFiles = Directory.GetFiles(_backupDirectory, "test_backup_*.csv");
            Assert.That(backupFiles.Length, Is.EqualTo(1), "Backup should be created in the specified backup location");
        }

        [Test]
        public void CreateBackup_WithCustomFormat_UsesSpecifiedFormat()
        {
            // Arrange
            var testFilePath = Path.Combine(_testDirectory, "test.csv");
            File.WriteAllText(testFilePath, "test,data\n1,2\n3,4");

            var project = CreateProjectWithBackupSettings(1, string.Empty, "backup-{filename}{extension}");

            // Act
            // Use reflection to access the internal BackupManager class
            var backupManagerType = typeof(Importer).Assembly.GetType("Orc.DbToCsv.BackupManager");
            var createBackupMethod = backupManagerType.GetMethod("CreateBackup", BindingFlags.Public | BindingFlags.Static);
            
            createBackupMethod.Invoke(null, new object[] { testFilePath, project });

            // Assert
            var backupFiles = Directory.GetFiles(_testDirectory, "backup-test.csv");
            Assert.That(backupFiles.Length, Is.EqualTo(1), "Backup should use the specified format");
        }

        private Project CreateProjectWithBackupSettings(int backupFileCount, string backupLocation, string backupFormat)
        {
            var project = new Project();
            project.BackupFileCount.Value = backupFileCount;
            project.BackupLocation.Value = backupLocation;
            project.BackupFormat.Value = backupFormat;
            return project;
        }
    }
}