namespace Orc.DbToCsv.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using NUnit.Framework;

    [TestFixture]
    public class ImporterFileRestorationTests
    {
        private string _testDirectory;

        [SetUp]
        public void Setup()
        {
            // Create a unique test directory
            _testDirectory = Path.Combine(Path.GetTempPath(), "DbToCsvRestorationTests_" + Guid.NewGuid().ToString("N"));
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
        public void ReplaceOriginalWithTemp_ShouldRestoreOriginalFile_WhenMoveOperationFails()
        {
            // Arrange
            var originalFilePath = Path.Combine(_testDirectory, "original.csv");
            var tempFilePath = Path.Combine(_testDirectory, "temp.csv");
            var expectedContent = "original,file,content\n1,2,3";
            
            // Create test files
            File.WriteAllText(originalFilePath, expectedContent);
            File.WriteAllText(tempFilePath, "temp,file,content\n4,5,6");
            
            // Create a condition where the move operation will fail
            // We'll simulate this by making the destination file read-only
            File.SetAttributes(originalFilePath, FileAttributes.ReadOnly);
            
            var project = new Project();
            project.BackupFileCount.Value = 1; // Keep 1 backup to test restoration
            
            // Act & Assert
            // Use reflection to access the private ReplaceOriginalWithTemp method
            var importerType = typeof(Importer);
            var replaceMethod = importerType.GetMethod("ReplaceOriginalWithTemp",
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // When using reflection, exceptions are wrapped in TargetInvocationException
            // This should throw an exception, but should also restore the original file
            var ex = Assert.Throws<TargetInvocationException>(() => {
                replaceMethod.Invoke(null, new object[] { originalFilePath, tempFilePath, _testDirectory, project });
            });
            
            // Make sure the inner exception is an IOException as expected
            Assert.That(ex.InnerException, Is.TypeOf<IOException>());
            
            // Verify the original file still exists and has the original content
            Assert.That(File.Exists(originalFilePath), Is.True, "Original file should still exist");
            
            // Remove read-only attribute so we can read the file
            File.SetAttributes(originalFilePath, FileAttributes.Normal);
            var actualContent = File.ReadAllText(originalFilePath);
            Assert.That(actualContent, Is.EqualTo(expectedContent), "Original file content should be restored");
            
            // Verify the temp file was cleaned up
            Assert.That(File.Exists(tempFilePath), Is.False, "Temporary file should be cleaned up");
        }

        [Test]
        public void ReplaceOriginalWithTemp_ShouldHandleBackupFailure_WithoutLosingOriginal()
        {
            // Arrange
            var originalFilePath = Path.Combine(_testDirectory, "original.csv");
            var tempFilePath = Path.Combine(_testDirectory, "temp.csv");
            var expectedContent = "original,file,content\n1,2,3";
            
            // Create test files
            File.WriteAllText(originalFilePath, expectedContent);
            File.WriteAllText(tempFilePath, "temp,file,content\n4,5,6");

            // Create a project with an invalid backup location to force a backup failure
            var project = new Project();
            project.BackupFileCount.Value = 1;
            project.BackupLocation.Value = Path.Combine(_testDirectory, "invalid\\path\\that\\does\\not\\exist");
            
            // Act & Assert
            // Use reflection to access the private ReplaceOriginalWithTemp method
            var importerType = typeof(Importer);
            var replaceMethod = importerType.GetMethod("ReplaceOriginalWithTemp", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // This should complete successfully even though the backup failed
            replaceMethod.Invoke(null, new object[] { originalFilePath, tempFilePath, _testDirectory, project });
            
            // Verify the original file was replaced with the temp file
            Assert.That(File.Exists(originalFilePath), Is.True, "Result file should exist");
            var actualContent = File.ReadAllText(originalFilePath);
            Assert.That(actualContent, Contains.Substring("temp,file,content"), 
                "Original file should be replaced with temp file content");
            
            // Verify the temp file was cleaned up
            Assert.That(File.Exists(tempFilePath), Is.False, "Temporary file should be cleaned up");
        }
    }
}