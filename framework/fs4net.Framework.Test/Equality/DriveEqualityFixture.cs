using NUnit.Framework;

namespace fs4net.Framework.Test.Equality
{
    [TestFixture]
    public class DriveEqualityFixture
    {
        private IFileSystem FileSystem { get; set; }


        [SetUp]
        public void SetUp()
        {
            FileSystem = new MockFileSystem();
        }


        [Test]
        public void Equal_Mapped_Drives_Are_Equal()
        {
            var driveA = FileSystem.DriveDescribing("c:");
            var driveB = FileSystem.DriveDescribing("c:");
            AssertEqualityEquals(driveA, driveB);
            AssertOperatorEquals(driveA, driveB);
        }

        [Test]
        public void Different_Mapped_Drives_Are_Not_Equal()
        {
            var driveA = FileSystem.DriveDescribing("c:");
            var driveB = FileSystem.DriveDescribing("d:");
            AssertEqualityNotEquals(driveA, driveB);
            AssertOperatorNotEquals(driveA, driveB);
        }

        [Test]
        public void Equal_Network_Drives_Are_Equal()
        {
            var driveA = FileSystem.DriveDescribing(@"\\network\drive");
            var driveB = FileSystem.DriveDescribing(@"\\network\drive");
            AssertEqualityEquals(driveA, driveB);
            AssertOperatorEquals(driveA, driveB);
        }

        [Test]
        public void Different_Network_Host_Names_In_Drives_Are_Not_Equal()
        {
            var driveA = FileSystem.DriveDescribing(@"\\network\drive");
            var driveB = FileSystem.DriveDescribing(@"\\worknet\drive");
            AssertEqualityNotEquals(driveA, driveB);
            AssertOperatorNotEquals(driveA, driveB);
        }

        [Test]
        public void Different_Network_Share_Names_In_Drives_Are_Not_Equal()
        {
            var driveA = FileSystem.DriveDescribing(@"\\network\drive");
            var driveB = FileSystem.DriveDescribing(@"\\network\share");
            AssertEqualityNotEquals(driveA, driveB);
            AssertOperatorNotEquals(driveA, driveB);
        }

        [Test]
        public void Drive_And_RootedDirectory_Drive_Are_Equal()
        {
            var drive = FileSystem.DriveDescribing("c:");
            var driveAsDirectoty = FileSystem.DirectoryDescribing(@"c:");
            AssertEqualityEquals(drive, driveAsDirectoty);
        }

        [Test]
        public void Drives_On_Different_FileSystems_Are_Not_Equal()
        {
            var driveA = FileSystem.DriveDescribing("c:");
            var driveB = new MockFileSystem().DriveDescribing("c:");
            AssertEqualityNotEquals(driveA, driveB);
            AssertOperatorNotEquals(driveA, driveB);
        }

        [Test]
        public void Comparison_Is_Case_Insensitive()
        {
            var driveA = FileSystem.DriveDescribing("c:");
            var driveB = FileSystem.DriveDescribing("C:");
            AssertEqualityEquals(driveA, driveB);
            AssertOperatorEquals(driveA, driveB);
        }

        private static void AssertEqualityEquals(Drive left, object right)
        {
            Assert.That(left.Equals(right), Is.True, string.Format("'{0}'.Equals('{1}')", left, right));
            Assert.That(left.GetHashCode(), Is.EqualTo(right.GetHashCode()), string.Format("'{0}'.GetHashCode() == '{1}'.GetHashCode()", left, right));
        }

        private static void AssertEqualityNotEquals(Drive left, object right)
        {
            Assert.That(left.Equals(right), Is.False, string.Format("! '{0}'.Equals('{1}')", left, right));
            Assert.That(left.GetHashCode(), Is.Not.EqualTo(right.GetHashCode()), string.Format("'{0}'.GetHashCode() != '{1}'.GetHashCode()", left, right));
        }

        private static void AssertOperatorEquals(Drive left, Drive right)
        {
            Assert.That(left == right, Is.True, string.Format("'{0}' == '{1}'", left, right));
            Assert.That(left != right, Is.False, string.Format("'{0}' != '{1}'", left, right));
        }

        private static void AssertOperatorNotEquals(Drive left, Drive right)
        {
            Assert.That(left == right, Is.False, string.Format("! '{0}' == '{1}'", left, right));
            Assert.That(left != right, Is.True, string.Format("! '{0}' != '{1}'", left, right));
        }
    }
}