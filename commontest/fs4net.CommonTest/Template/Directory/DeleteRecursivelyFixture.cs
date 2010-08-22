using System.IO;
using fs4net.Framework;
using NUnit.Framework;

namespace fs4net.CommonTest.Template.Directory
{
    [TestFixture]
    public abstract class DeleteRecursivelyFixture : PopulatedFileSystem
    {
        [Test]
        public void Delete_Existing_SubFolders_And_Files()
        {
            ParentOfExistingLeafDirectory.DeleteRecursively();
            Assert.That(ParentOfExistingLeafDirectory.Exists(), Is.False);
            Assert.That(ExistingLeafDirectory.Exists(), Is.False);
            Assert.That(ExistingFile.Exists(), Is.False);
        }

        [Test]
        public void Delete_NonExisting_Directory()
        {
            Assert.DoesNotThrow(() => NonExistingDirectory.DeleteRecursively());
            Assert.That(NonExistingDirectory.Exists(), Is.False);
        }

        [Test]
        public void Delete_Directory_That_Denotes_A_File_Throws()
        {
            var fileAsDirectory = FileSystem.CreateDirectoryDescribing(ExistingFile.PathAsString);
            Assert.Throws<IOException>(() => fileAsDirectory.DeleteRecursively());
            Assert.That(ExistingFile.Exists(), Is.True);
        }

        [Test]
        public void Delete_Directory_On_NonExisting_Drive_Throws()
        {
            var toBeCreated = FileSystem.CreateDirectoryDescribing(@"z:\drive\does\no\exist");
            Assert.DoesNotThrow(() => toBeCreated.DeleteRecursively());
        }

        // TODO: Access denied
        // e.g. Current Directory, file is open, directory is in use, read-only
    }
}