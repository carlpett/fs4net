using fs4net.Framework;
using NUnit.Framework;
using Template = fs4net.CommonTest.Template;

namespace fs4net.Memory.Test.File
{
    [TestFixture]
    public class CreateModifyStreamFixture : Template.File.CreateModifyStreamFixture
    {
        protected override IFileSystem CreateFileSystem()
        {
            return FileSystemFactory.CreateFileSystemWithDrives();
        }

        protected override void DisposeFileSystem(IFileSystem fileSystem)
        {
            ((MemoryFileSystem)fileSystem).Dispose();
        }
    }
}
