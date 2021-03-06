using NUnit.Framework;

namespace fs4net.Framework.Test.PathAsString
{
    [TestFixture]
    public class RelativeCanonicalPathAsStringFixture
    {
        private static readonly string[][] OriginalAndExpected =
            {
                new[] { @"standard\case\to\fileOrDirectory.txt", @"standard\case\to\fileOrDirectory.txt" },
                new[] { @"single\.\dots\to\.\fileOrDirectory.txt", @"single\dots\to\fileOrDirectory.txt" },
                new[] { @"double\..\dots\to\..\fileOrDirectory.txt", @"dots\fileOrDirectory.txt" },
                new[] { @".\starting\with\single\dot\to\fileOrDirectory.txt", @"starting\with\single\dot\to\fileOrDirectory.txt" },
                new[] { @"..\starting\with\double\dots\to\fileOrDirectory.txt", @"..\starting\with\double\dots\to\fileOrDirectory.txt" },
            };


        [Test, TestCaseSource("OriginalAndExpected")]
        public void RelativeFile_Canonical_PathAsString(string original, string expected)
        {
            Assert.That(RelativeFile.FromString(original).AsCanonical().PathAsString, Is.EqualTo(expected));
        }

        [Test, TestCaseSource("OriginalAndExpected")]
        public void RelativeDirectory_Canonical_PathAsString(string original, string expected)
        {
            Assert.That(RelativeDirectory.FromString(original).AsCanonical().PathAsString, Is.EqualTo(expected));
        }


        [Test]
        public void RelativeDirectory_Not_Ending_With_Backslash_Is_Intact_In_Canonical_Form()
        {
            Assert.That(RelativeDirectory.FromString(@"path\to").AsCanonical().PathAsString, Is.EqualTo(@"path\to"));
        }

        [Test]
        public void RelativeDirectory_Ending_Dot_Removed_In_Canonical_Form()
        {
            Assert.That(RelativeDirectory.FromString(@"path\to\.").AsCanonical().PathAsString, Is.EqualTo(@"path\to"));
        }

        [Test]
        public void RelativeDirectory_Ending_DoubleDots_Removed_In_Canonical_Form()
        {
            Assert.That(RelativeDirectory.FromString(@"path\to\..").AsCanonical().PathAsString, Is.EqualTo(@"path"));
        }
    }
}