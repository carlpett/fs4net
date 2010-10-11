using System;
using System.IO;
using fs4net.Framework;
using NUnit.Framework;

namespace fs4net.TestTemplates.File
{
    public abstract class CreateModifyStreamFixture : PopulatedFileSystem
    {
        [Test]
        public void Append_To_NonExisting_File()
        {
            var file = ExistingEmptyDirectory + RelativeFile.FromString("newfile.txt");
            file.OverwriteText("Paradise");
            Assert.That(file.ReadText(), Is.EqualTo("Paradise"));
        }

        [Test]
        public void Append_To_NonExisting_File_In_NonExisting_Directory()
        {
            var file = NonExistingDirectory + RelativeFile.FromString("nonexisting.txt");
            Assert.Throws<DirectoryNotFoundException>(() => file.OverwriteText("Willow"));
            Assert.That(NonExistingDirectory.Exists(), Is.False);
            Assert.That(file.Exists(), Is.False);
        }

        [Test]
        public void Append_To_File_That_Is_An_Existing_Directory()
        {
            var file = FileSystem.FileDescribing(ExistingEmptyDirectory.PathAsString);
            Assert.Throws<UnauthorizedAccessException>(() => file.OverwriteText("Nota Bossa"));
        }

        [Test]
        public void Append_To_File_On_NonExisting_Drive()
        {
            var file = NonExistingDrive + RelativeFile.FromString(@"path\to\nonexisting.txt");
            Assert.Throws<DirectoryNotFoundException>(() => file.OverwriteText("Nah nah nah"));
            Assert.That(file.Exists(), Is.False);
        }

        [Test]
        public void Append_To_Existing_File()
        {
            const string appendedText = "Farruca";
            using (var stream = ExistingFile.CreateModifyStream())
            using (var writer = new StreamWriter(stream))
            {
                stream.Seek(0, SeekOrigin.End);
                writer.Write(appendedText);
            }
            Assert.That(ExistingFile.ReadText(), Is.EqualTo(ExistingFileContents + appendedText));
        }

        [Test]
        public void Overwrite_Beginning_Of_Existing_File()
        {
            ExistingFile.OverwriteText("Dec");
            Assert.That(ExistingFile.ReadText(), Is.EqualTo("Deciembre"));
        }

        [Test]
        public void Overwrite_Existing_File_By_Append()
        {
            const string newText = "It was the third of september";
            ExistingFile.OverwriteText(newText);
            Assert.That(ExistingFile.ReadText(), Is.EqualTo(newText));
        }

        [Test]
        public void Read_Does_Not_Read_Written_Contents()
        {
            var newFile = ExistingEmptyDirectory + RelativeFile.FromString("newfile.txt");
            const string newContents = "Keep on Movin";
            using (var stream = newFile.CreateModifyStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(newContents);
                writer.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream);
                string contents = reader.ReadToEnd();

                Assert.That(contents, Is.EqualTo(newContents));
            }
            Assert.That(newFile.ReadText(), Is.EqualTo(newContents));
        }

        // TODO: Access denied
    }

    internal static class RootedFileTestUtilities
    {
        public static void OverwriteText(this RootedFile me, string text)
        {
            using (var stream = me.CreateModifyStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(text);
            }
        }
    }
}