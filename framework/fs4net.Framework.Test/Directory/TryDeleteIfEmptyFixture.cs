using NUnit.Framework;
using Template = fs4net.CommonTest.Template;

namespace fs4net.Framework.Test.Directory
{
    [TestFixture]
    public class TryDeleteIfEmptyFixture : Template.Directory.TryDeleteIfEmptyFixture
    {
        protected override IFileSystem CreateFileSystem()
        {
            return new FileSystem();
        }
    }
}