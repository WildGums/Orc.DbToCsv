namespace Orc.DbToCsv.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class ImporterTests
    {
        private string _testDirectory;

        [SetUp]
        public void Setup()
        {
            // Create a unique test directory
            _testDirectory = Path.Combine(Path.GetTempPath(), "DbToCsvImporterTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testDirectory);
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
        public void ReplaceOriginalWithTemp_ShouldCleanupTempFileOnError()
        {
            // Arrange
            var originalFilePath = Path.Combine(_testDirectory, "original.csv");
            var tempFilePath = Path.Combine(_testDirectory, "temp.csv");
            
            // Create test files
            File.WriteAllText(tempFilePath, "test,data\n1,2\n3,4");
            
            // Make the destination path invalid by creating a readonly directory with the same name
            File.Delete(tempFilePath);
            Directory.CreateDirectory(tempFilePath);
            
            var project = new Project();
            project.BackupFileCount.Value = 0; // Don't keep backups

            // Act
            // Use reflection to access the private ReplaceOriginalWithTemp method
            var importerType = typeof(Importer);
            var replaceMethod = importerType.GetMethod("ReplaceOriginalWithTemp", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Assert - This should throw an exception, but should clean up the temp file
            var ex = Assert.Throws<TargetInvocationException>(() => {
                replaceMethod.Invoke(null, new object[] { originalFilePath, tempFilePath, _testDirectory, project });
            });
            
            // Make sure the inner exception is an IOException as expected
            Assert.That(ex.InnerException, Is.TypeOf<IOException>());
            
            // Verify the temp file was cleaned up (removed or attempted to be removed)
            var tempFiles = Directory.GetFiles(_testDirectory, "*temp*");
            Assert.That(tempFiles.Length, Is.EqualTo(0), "Temporary files should be cleaned up even when operation fails");
        }

        [Test]
        public void CreateTempFileName_ShouldCreateUniqueNames()
        {
            // Arrange
            var originalFilePath = Path.Combine(_testDirectory, "test.csv");
            
            // Act
            // Use reflection to access the private CreateTempFileName method
            var importerType = typeof(Importer);
            var createTempMethod = importerType.GetMethod("CreateTempFileName", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            var tempName1 = createTempMethod.Invoke(null, new object[] { originalFilePath, _testDirectory }) as string;
            var tempName2 = createTempMethod.Invoke(null, new object[] { originalFilePath, _testDirectory }) as string;
            
            // Assert
            Assert.That(tempName1, Is.Not.EqualTo(tempName2), "Temporary filenames should be unique");
            Assert.That(tempName1, Does.Contain("_temp_"), "Temporary filenames should contain '_temp_' marker");
            Assert.That(Path.GetExtension(tempName1), Is.EqualTo(".csv"), "Temporary files should maintain the original extension");
        }

        [Test]
        public void CleanupTempFile_ShouldRemoveExistingTempFile()
        {
            // Arrange
            var tempFilePath = Path.Combine(_testDirectory, "temp_file.csv");
            File.WriteAllText(tempFilePath, "test,data\n1,2\n3,4");
            
            // Act
            // Use reflection to access the private CleanupTempFile method
            var importerType = typeof(Importer);
            var cleanupMethod = importerType.GetMethod("CleanupTempFile", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            cleanupMethod.Invoke(null, new object[] { tempFilePath });
            
            // Assert
            Assert.That(File.Exists(tempFilePath), Is.False, "Temporary file should be removed");
        }

        [Test]
        public void CleanupTempFile_ShouldHandleNonExistentFile()
        {
            // Arrange
            var nonExistentFilePath = Path.Combine(_testDirectory, "non_existent_file.csv");
            
            // Act & Assert - Should not throw
            var importerType = typeof(Importer);
            var cleanupMethod = importerType.GetMethod("CleanupTempFile", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            Assert.DoesNotThrow(() => {
                cleanupMethod.Invoke(null, new object[] { nonExistentFilePath });
            });
        }
    }
}