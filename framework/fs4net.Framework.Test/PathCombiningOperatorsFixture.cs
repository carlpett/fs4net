using System;
using NUnit.Framework;

namespace fs4net.Framework.Test
{
    [TestFixture]
    public class PathCombiningOperatorsFixture
    {
        private static IFileSystem _fileSystem = new MockFileSystem();

        // Original data
        private static RootedDirectory Drive_WithoutBackslash = _fileSystem.CreateDirectoryDescribing(@"c:");
        private static RootedDirectory Drive_WithBackslash = _fileSystem.CreateDirectoryDescribing(@"c:\");
        private static RootedDirectory RootedDirectory_WithoutBackslash = _fileSystem.CreateDirectoryDescribing(@"c:\path\is");
        private static RootedDirectory RootedDirectory_WithBackslash = _fileSystem.CreateDirectoryDescribing(@"c:\path\is\");

        private static RelativeDirectory RelativeDirectory_WithStartBackslash_WithoutEndBackslash = RelativeDirectory.FromString(@"\relative\to");
        private static RelativeDirectory RelativeDirectory_WithoutStartBackslash_WithEndBackslash = RelativeDirectory.FromString(@"relative\to\");

        private static RelativeFile FilenameOnly_WithoutBackslash = RelativeFile.FromString(@"file.txt");
        private static RelativeFile FilenameOnly_WithBackslash = RelativeFile.FromString(@"\file.txt");
        private static RelativeFile RelativeFile_WithoutBackslash = RelativeFile.FromString(@"my\file.txt");
        private static RelativeFile RelativeFile_WithBackslash = RelativeFile.FromString(@"\my\file.txt");


        // Expected data
        private static RootedDirectory DriveAndRelativeDirectory_WithoutEndBackslash = _fileSystem.CreateDirectoryDescribing(@"c:\relative\to");
        private static RootedDirectory DriveAndRelativeDirectory_WithEndBackslash = _fileSystem.CreateDirectoryDescribing(@"c:\relative\to\");
        private static RootedDirectory RootedDirectoryAndRelativeDirectory_WithoutEndBackslash = _fileSystem.CreateDirectoryDescribing(@"c:\path\is\relative\to");
        private static RootedDirectory RootedDirectoryAndRelativeDirectory_WithEndBackslash = _fileSystem.CreateDirectoryDescribing(@"c:\path\is\relative\to\");

        private static RootedFile DriveAndFilename = _fileSystem.CreateFileDescribing(@"c:\file.txt");
        private static RootedFile RootedDirectoryAndFilename = _fileSystem.CreateFileDescribing(@"c:\path\is\file.txt");
        private static RootedFile DriveAndRelativeFile = _fileSystem.CreateFileDescribing(@"c:\my\file.txt");
        private static RootedFile RootedDirectoryAndRelativeFile = _fileSystem.CreateFileDescribing(@"c:\path\is\my\file.txt");

        private static RelativeFile RelativeDirectoryAndFilename_WithStartBackslash = RelativeFile.FromString(@"\relative\to\file.txt");
        private static RelativeFile RelativeDirectoryAndFilename_WithoutStartBackslash = RelativeFile.FromString(@"relative\to\file.txt");
        private static RelativeFile RelativeDirectoryAndRelativeFile_WithStartBackslash = RelativeFile.FromString(@"\relative\to\my\file.txt");
        private static RelativeFile RelativeDirectoryAndRelativeFile_WithoutStartBackslash = RelativeFile.FromString(@"relative\to\my\file.txt");
        


        private static readonly object[][] RootedDirectory_And_RelativeDirectory =
            {
                new object[] { Drive_WithoutBackslash, RelativeDirectory_WithStartBackslash_WithoutEndBackslash, DriveAndRelativeDirectory_WithoutEndBackslash },
                new object[] { Drive_WithBackslash, RelativeDirectory_WithStartBackslash_WithoutEndBackslash, DriveAndRelativeDirectory_WithoutEndBackslash },
                new object[] { RootedDirectory_WithoutBackslash, RelativeDirectory_WithStartBackslash_WithoutEndBackslash, RootedDirectoryAndRelativeDirectory_WithoutEndBackslash },
                new object[] { RootedDirectory_WithBackslash, RelativeDirectory_WithStartBackslash_WithoutEndBackslash, RootedDirectoryAndRelativeDirectory_WithoutEndBackslash },

                new object[] { Drive_WithoutBackslash, RelativeDirectory_WithoutStartBackslash_WithEndBackslash, DriveAndRelativeDirectory_WithEndBackslash },
                new object[] { Drive_WithBackslash, RelativeDirectory_WithoutStartBackslash_WithEndBackslash, DriveAndRelativeDirectory_WithEndBackslash },
                new object[] { RootedDirectory_WithoutBackslash, RelativeDirectory_WithoutStartBackslash_WithEndBackslash, RootedDirectoryAndRelativeDirectory_WithEndBackslash },
                new object[] { RootedDirectory_WithBackslash, RelativeDirectory_WithoutStartBackslash_WithEndBackslash, RootedDirectoryAndRelativeDirectory_WithEndBackslash },
            };

        [Test]
        public void Combine_RootedDirectory_And_RelativeDirectory()
        {
            RootedDirectory_And_RelativeDirectory.ForEach(Combine_RootedDirectory_And_RelativeDirectory);
        }

        public void Combine_RootedDirectory_And_RelativeDirectory(object[] testData)
        {
            Combine_RootedDirectory_And_RelativeDirectory((RootedDirectory) testData[0], (RelativeDirectory) testData[1], (RootedDirectory) testData[2]);
        }

        [Test, TestCaseSource("RootedDirectory_And_RelativeDirectory")]
        private void Combine_RootedDirectory_And_RelativeDirectory(RootedDirectory lhs, RelativeDirectory rhs, RootedDirectory expected)
        {
            Assert.That((lhs + rhs).PathAsString, Is.EqualTo(expected.PathAsString), string.Format("for '{0}' + '{1}'", lhs, rhs));
        }


        private static readonly object[][] RootedDirectory_And_RelativeFile =
            {
                new object[] { Drive_WithoutBackslash, FilenameOnly_WithoutBackslash, DriveAndFilename },
                new object[] { Drive_WithBackslash, FilenameOnly_WithoutBackslash, DriveAndFilename },
                new object[] { RootedDirectory_WithoutBackslash, FilenameOnly_WithoutBackslash, RootedDirectoryAndFilename },
                new object[] { RootedDirectory_WithBackslash, FilenameOnly_WithoutBackslash, RootedDirectoryAndFilename },

                new object[] { Drive_WithoutBackslash, FilenameOnly_WithBackslash, DriveAndFilename },
                new object[] { Drive_WithBackslash, FilenameOnly_WithBackslash, DriveAndFilename },
                new object[] { RootedDirectory_WithoutBackslash, FilenameOnly_WithBackslash, RootedDirectoryAndFilename },
                new object[] { RootedDirectory_WithBackslash, FilenameOnly_WithBackslash, RootedDirectoryAndFilename },

                new object[] { Drive_WithoutBackslash, RelativeFile_WithoutBackslash, DriveAndRelativeFile },
                new object[] { Drive_WithBackslash, RelativeFile_WithoutBackslash, DriveAndRelativeFile },
                new object[] { RootedDirectory_WithoutBackslash, RelativeFile_WithoutBackslash, RootedDirectoryAndRelativeFile },
                new object[] { RootedDirectory_WithBackslash, RelativeFile_WithoutBackslash, RootedDirectoryAndRelativeFile },

                new object[] { Drive_WithoutBackslash, RelativeFile_WithBackslash, DriveAndRelativeFile },
                new object[] { Drive_WithBackslash, RelativeFile_WithBackslash, DriveAndRelativeFile },
                new object[] { RootedDirectory_WithoutBackslash, RelativeFile_WithBackslash, RootedDirectoryAndRelativeFile },
                new object[] { RootedDirectory_WithBackslash, RelativeFile_WithBackslash, RootedDirectoryAndRelativeFile },
            };

        [Test]
        public void Combine_RootedDirectory_And_RelativeFile()
        {
            RootedDirectory_And_RelativeFile.ForEach(Combine_RootedDirectory_And_RelativeFile);
        }

        public void Combine_RootedDirectory_And_RelativeFile(object[] testData)
        {
            Combine_RootedDirectory_And_RelativeFile((RootedDirectory)testData[0], (RelativeFile)testData[1], (RootedFile)testData[2]);
        }

        [Test, TestCaseSource("RootedDirectory_And_RelativeFile")]
        private void Combine_RootedDirectory_And_RelativeFile(RootedDirectory lhs, RelativeFile rhs, RootedFile expected)
        {
            Assert.That((lhs + rhs).PathAsString, Is.EqualTo(expected.PathAsString), string.Format("for '{0}' + '{1}'", lhs, rhs));
        }


        private static readonly object[][] RelativeDirectory_And_RelativeFile =
            {
                new object[] { RelativeDirectory_WithStartBackslash_WithoutEndBackslash, FilenameOnly_WithoutBackslash, RelativeDirectoryAndFilename_WithStartBackslash },
                new object[] { RelativeDirectory_WithoutStartBackslash_WithEndBackslash, FilenameOnly_WithoutBackslash, RelativeDirectoryAndFilename_WithoutStartBackslash },

                new object[] { RelativeDirectory_WithStartBackslash_WithoutEndBackslash, FilenameOnly_WithBackslash, RelativeDirectoryAndFilename_WithStartBackslash },
                new object[] { RelativeDirectory_WithoutStartBackslash_WithEndBackslash, FilenameOnly_WithBackslash, RelativeDirectoryAndFilename_WithoutStartBackslash },

                new object[] { RelativeDirectory_WithStartBackslash_WithoutEndBackslash, RelativeFile_WithoutBackslash, RelativeDirectoryAndRelativeFile_WithStartBackslash },
                new object[] { RelativeDirectory_WithoutStartBackslash_WithEndBackslash, RelativeFile_WithoutBackslash, RelativeDirectoryAndRelativeFile_WithoutStartBackslash },

                new object[] { RelativeDirectory_WithStartBackslash_WithoutEndBackslash, RelativeFile_WithBackslash, RelativeDirectoryAndRelativeFile_WithStartBackslash },
                new object[] { RelativeDirectory_WithoutStartBackslash_WithEndBackslash, RelativeFile_WithBackslash, RelativeDirectoryAndRelativeFile_WithoutStartBackslash },
            };

        [Test]
        public void Combine_RelativeDirectory_And_RelativeFile()
        {
            RelativeDirectory_And_RelativeFile.ForEach(Combine_RelativeDirectory_And_RelativeFile);
        }

        public void Combine_RelativeDirectory_And_RelativeFile(object[] testData)
        {
            Combine_RelativeDirectory_And_RelativeFile((RelativeDirectory)testData[0], (RelativeFile)testData[1], (RelativeFile)testData[2]);
        }

        [Test, TestCaseSource("RelativeDirectory_And_RelativeFile")]
        private void Combine_RelativeDirectory_And_RelativeFile(RelativeDirectory lhs, RelativeFile rhs, RelativeFile expected)
        {
            Assert.That((lhs + rhs).PathAsString, Is.EqualTo(expected.PathAsString), string.Format("for '{0}' + '{1}'", lhs, rhs));
        }

        // TODO: RelativeFile and RelativeFile
    }
}