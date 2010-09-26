using System;
using System.IO;
using fs4net.Framework;
using NUnit.Framework;

namespace fs4net.TestTemplates.File
{
    public abstract class MoveToFixture : PopulatedFileSystem
    {
        [Test]
        public void Move_File_To_Same_Directory_But_Different_Name()
        {
            var source = ExistingFile;
            var destination = source.WithFileName(FileName.FromString("new name.dat"));

            source.MoveTo(destination);

            Assert.That(source.Exists(), Is.False);
            Assert.That(destination.Exists(), Is.True);
        }

        [Test]
        public void Move_File_To_Another_Directory()
        {
            var source = ExistingFile;
            var destination = ExistingEmptyDirectory + ExistingFile.FileName();

            source.MoveTo(destination);

            Assert.That(source.Exists(), Is.False);
            Assert.That(destination.Exists(), Is.True);
        }

        [Test]
        public void Move_NonExisting_File_Throws()
        {
            var source = NonExistingFile;
            var destination = ExistingLeafDirectory + FileName.FromString("new name.dat");

            Assert.Throws<FileNotFoundException>(() => source.MoveTo(destination));
        }

        [Test]
        public void Move_Directory_As_File_Throws()
        {
            var source = FileSystem.CreateFileDescribing(ExistingEmptyDirectory.PathAsString);
            var destination = ExistingLeafDirectory + RelativeFile.FromString("new name.dat");

            Assert.Throws<FileNotFoundException>(() => source.MoveTo(destination));
        }

        [Test]
        public void Move_File_Into_NonExisting_Directory_Throws()
        {
            var source = ExistingFile;
            var destination = NonExistingDirectory + source.FileName();

            Assert.Throws<DirectoryNotFoundException>(() => source.MoveTo(destination));
            Assert.That(source.Exists(), Is.True);
        }

        [Test]
        public void Move_File_To_Existing_File_Throws()
        {
            var source = ExistingFile;
            var destination = ExistingFile2;

            Assert.Throws<IOException>(() => source.MoveTo(destination));
            Assert.That(source.Exists(), Is.True);
        }

        [Test]
        public void Move_File_To_Name_Occupied_By_Existing_Directory_Throws()
        {
            var source = ExistingFile;
            var destination = FileSystem.CreateFileDescribing(ExistingEmptyDirectory.PathAsString);

            Assert.Throws<IOException>(() => source.MoveTo(destination));
            Assert.That(source.Exists(), Is.True);
        }

        [Test]
        public void Move_File_To_Itself_Throws()
        {
            // System.IO.File.Move() allows this but not System.IO.Directory.Move()...
            // I prefer to be consequent
            var source = ExistingFile;
            var destination = ExistingFile;

            Assert.Throws<IOException>(() => source.MoveTo(destination));
            Assert.That(source.Exists(), Is.True);
        }

        [Test]
        public void Move_File_To_Another_Drive_Throws()
        {
            // System.IO.File.Move() allows this but not System.IO.Directory.Move()...
            // I prefer to be consequent
            var source = ExistingFile;
            var destination = FileSystem.CreateFileDescribing(@"d:\another drive.txt");

            Assert.Throws<IOException>(() => source.MoveTo(destination));
            Assert.That(source.Exists(), Is.True);
        }

        [Test]
        public void Move_Between_FileSystems_Throws()
        {
            var source = ExistingFile;
            var destination = CreateFileSystem().CreateFileDescribing(source.WithFileName(FileName.FromString("new name.dat")).PathAsString);

            Assert.Throws<InvalidOperationException>(() => source.MoveTo(destination));
            Assert.That(source.Exists(), Is.True);
        }

        // TODO: Access denied (source and destination)
    }
}