using System.IO;
using fs4net.Framework;
using NUnit.Framework;

namespace fs4net.TestTemplates.Directory
{
    [TestFixture]
    public abstract class DeleteIfEmptyFixture : PopulatedFileSystem
    {
        [Test]
        public void Delete_Empty_Directory()
        {
            ExistingEmptyDirectory.DeleteIfEmpty();
            Assert.That(ExistingEmptyDirectory.Exists(), Is.False);
        }

        [Test]
        public void Delete_Directory_Containing_File_Throws()
        {
            Should.Throw<IOException>(() => ExistingLeafDirectory.DeleteIfEmpty());
            Assert.That(ExistingLeafDirectory.Exists(), Is.True);
            Assert.That(ExistingFile.Exists(), Is.True);
        }

        [Test]
        public void Delete_NonExisting_Directory_Throws()
        {
            var toBeDeleted = (ExistingLeafDirectory + RelativeDirectory.FromString("nonexisting"));
            Should.Throw<DirectoryNotFoundException>(() => toBeDeleted.DeleteIfEmpty()); // Not sure if I like this, but that's how .NET works...
            Assert.That(toBeDeleted.Exists(), Is.False);
        }

        [Test]
        public void Delete_NonExisting_Folder_In_NonExisting_Directory_Throws()
        {
            var toBeDeleted = (NonExistingDirectory + RelativeDirectory.FromString("nonexisting"));
            Should.Throw<DirectoryNotFoundException>(() => toBeDeleted.DeleteIfEmpty());
        }

        [Test]
        public void Delete_Directory_That_Is_A_File_Throws()
        {
            var fileAsDirectory = FileSystem.DirectoryDescribing(ExistingFile.PathAsString);
            Should.Throw<IOException>(() => fileAsDirectory.DeleteIfEmpty());
            Assert.That(ExistingFile.Exists(), Is.True);
        }

        [Test]
        [Ignore("Don't know what drives exist on the CI")]
        public void Delete_Directory_On_NonExisting_Drive_Throws()
        {
            var toBeDeleted = NonExistingDrive + RelativeDirectory.FromString(@"drive\does\not\exist");
            Should.Throw<DirectoryNotFoundException>(() => toBeDeleted.DeleteIfEmpty());
        }


        // TODO: Access denied
        // e.g. Current Directory, file is open, directory is in use, read-only
    }
}