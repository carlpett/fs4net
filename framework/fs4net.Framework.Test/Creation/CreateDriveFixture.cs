using System;
using NUnit.Framework;

namespace fs4net.Framework.Test.Creation
{
    [TestFixture]
    public class CreateDriveFixture
    {
        private IFileSystem _fileSystem;


        [SetUp]
        public void CreateMockFileSystem()
        {
            _fileSystem = new MockFileSystem();
        }


        [Test]
        public void Throws_If_FileSystem_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new Drive(null, "c:"));
        }
        
        [Test]
        public void Throws_If_Name_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => _fileSystem.DriveDescribing(null));
        }

        [Test]
        public void Throws_If_Name_Is_Empty()
        {
            Assert.Throws<InvalidPathException>(() => _fileSystem.DriveDescribing(string.Empty));
        }


        private static readonly string[] InvalidDrives =
            {
                @"c",
                @"cc",
                @"c:\",
                @"�:",
                @"c:c",
                @"\\colon:in\network\name",
                @"\\colon\in:share\name",
                @"\\network",
                @"\\network\",
                @"\\network\share\",
            };

        [Test]
        public void Throws_If_Is_Invalid()
        {
            InvalidDrives.ForEach(Throws_If_Is_Invalid);
        }

        [Test, TestCaseSource("InvalidDrives")]
        public void Throws_If_Is_Invalid(string invalidDrive)
        {
            AssertThrowsInvalidPathExceptionFor(invalidDrive);
        }


        private static readonly string[] ValidDrives =
            {
                @"c:",
                @"C:",
                @"d:",
                @"z:",
                @"\\network\name",
            };

        [Test]
        public void Create_With_Valid_Drive()
        {
            ValidDrives.ForEach(Create_With_Valid_Drive);
        }

        [Test, TestCaseSource("ValidDrives")]
        public void Create_With_Valid_Drive(string validDrive)
        {
            Assert.DoesNotThrow(() => _fileSystem.DriveDescribing(validDrive), string.Format("for '{0}'", validDrive));
        }


        private void AssertThrowsInvalidPathExceptionFor(string invalidDrive)
        {
            Assert.Throws<InvalidPathException>(() => _fileSystem.DriveDescribing(invalidDrive), string.Format("for '{0}'", invalidDrive));
        }
    }
}