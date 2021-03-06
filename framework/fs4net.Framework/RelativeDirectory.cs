using fs4net.Framework.Impl;

namespace fs4net.Framework
{
    /// <summary>
    /// Represents a relative path to a directory. It is relative, or non-rooted, meaning that that it does not start
    /// with a drive (e.g. c:, d:, \\network\drive, etc).
    /// </summary>
    public sealed class RelativeDirectory : IDirectory<RelativeDirectory>, IRelativeFileSystemItem<RelativeDirectory>
    {
        private readonly string _canonicalFullPath;

        private RelativeDirectory(string relativePath)
        {
            PathAsString = relativePath;
            _canonicalFullPath = new CanonicalPathBuilder(relativePath).BuildForRelativeDirectory();
        }

        #region Public Interface

        /// <summary>
        /// Initializes a new instance of the class on the specified path. The path may not end with a backslash.
        /// </summary>
        /// <param name="relativePath">A string specifying the path that the class should encapsulate.</param>
        /// <exception cref="System.ArgumentNullException">The specified path is null.</exception>
        /// <exception cref="fs4net.Framework.InvalidPathException">The specified path is invalid, e.g. it's empty,
        /// starts or ends with white space or contains one or more invalid characters.</exception>
        /// <exception cref="fs4net.Framework.RootedPathException">The specified path is rooted.</exception>
        public static RelativeDirectory FromString(string relativePath)
        {
            return new RelativeDirectory(relativePath);
        }

        public string PathAsString { get; private set; }

        public RelativeDirectory AsCanonical()
        {
            return new RelativeDirectory(_canonicalFullPath);
        }

        /// <summary>
        /// Concatenates the two descriptors into one and returns it.
        /// </summary>
        public static RelativeDirectory operator +(RelativeDirectory left, RelativeDirectory right)
        {
            return left.Append(right);
        }

        /// <summary>
        /// Concatenates the two descriptors into one and returns it.
        /// </summary>
        public static RelativeFile operator +(RelativeDirectory left, RelativeFile right)
        {
            return left.Append(right);
        }

        /// <summary>
        /// Concatenates the two descriptors into one and returns it.
        /// </summary>
        public static RelativeFile operator +(RelativeDirectory left, FileName right)
        {
            return left.Append(right);
        }

        #endregion // Public Interface

        #region Value Object

        /// <summary>
        /// Determines whether the specified instance denotes the same path as the current instance. The
        /// comparison is made using the canonical form, meaning that redundant "." and ".." have been removed.
        /// </summary>
        public bool Equals(RelativeDirectory other)
        {
            return this.DenotesSamePathAs(other);
        }

        public override bool Equals(object obj)
        {
            return this.DenotesSamePathAs(obj);
        }

        public override int GetHashCode()
        {
            return this.InternalGetHashCode();
        }

        /// <summary>
        /// Determines whether the left instance denotes the same path as the right instance. The
        /// comparison is made using the canonical form, meaning that redundant "." and ".." have been removed.
        /// </summary>
        public static bool operator ==(RelativeDirectory left, RelativeDirectory right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether the left instance denotes a different path than the right instance. The
        /// comparison is made using the canonical form, meaning that redundant "." and ".." have been removed.
        /// </summary>
        public static bool operator !=(RelativeDirectory left, RelativeDirectory right)
        {
            return !Equals(left, right);
        }

        #endregion // Value Object

        #region Debugging

        public override string ToString()
        {
            return PathAsString;
        }

        #endregion Debugging
    }

    public static class RelativeDirectoryExtensions
    {
        /// <summary>
        /// Concatenates the two descriptors into one and returns it.
        /// </summary>
        public static RelativeFile Append(this RelativeDirectory left, RelativeFile right)
        {
            ThrowHelper.ThrowIfNull(left, "left");
            ThrowHelper.ThrowIfNull(right, "right");
            if (left.PathAsString.IsEmpty()) return right;
            return RelativeFile.FromString(PathUtils.Combine(left.PathAsString, right.PathAsString));
        }

        /// <summary>
        /// Concatenates the two descriptors into one and returns it.
        /// </summary>
        public static RelativeFile Append(this RelativeDirectory left, FileName right)
        {
            ThrowHelper.ThrowIfNull(left, "left");
            ThrowHelper.ThrowIfNull(right, "right");
            if (left.PathAsString.IsEmpty()) return right.AsRelativeFile();
            return RelativeFile.FromString(PathUtils.Combine(left.PathAsString, right.PathAsString));
        }

        /// <summary>
        /// Concatenates the two descriptors into one and returns it.
        /// </summary>
        public static RelativeDirectory Append(this RelativeDirectory left, RelativeDirectory right)
        {
            ThrowHelper.ThrowIfNull(left, "left");
            ThrowHelper.ThrowIfNull(right, "right");
            if (left.PathAsString.IsEmpty()) return right;
            if (right.PathAsString.IsEmpty()) return left;
            return RelativeDirectory.FromString(PathUtils.Combine(left.PathAsString, right.PathAsString));
        }

        /// <summary>
        /// Returns the parent directory of the denoted item.
        /// </summary>
        public static RelativeDirectory Parent(this RelativeDirectory me)
        {
            ThrowHelper.ThrowIfNull(me, "me");
            return me.Append(RelativeDirectory.FromString("..")).AsCanonical();
        }

        /// <summary>
        /// Returns a relative descriptor containing the name of the leaf folder of this path.
        /// Example: LeafFolder("my\path\to") => "to".
        /// The method returns the leaf folder in the path's canonical form. This could be an empty directory or even
        /// "..".
        /// </summary>
        public static RelativeDirectory LeafFolder(this RelativeDirectory me)
        {
            ThrowHelper.ThrowIfNull(me, "me");
            var canonicalPath = me.AsCanonical().PathAsString;
            var lastBackslashIndex = canonicalPath.LastIndexOf('\\');
            return RelativeDirectory.FromString(canonicalPath.Substring(lastBackslashIndex + 1));
        }
    }
}