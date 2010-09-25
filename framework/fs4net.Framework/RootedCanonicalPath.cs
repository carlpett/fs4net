namespace fs4net.Framework
{
    public struct RootedCanonicalPath
    {
        public string FullPath { get; private set; }

        internal RootedCanonicalPath(string fullPath) : this()
        {
            FullPath = fullPath;
        }

        #region Value Object

        public bool Equals(RootedCanonicalPath other)
        {
            return Equals(other.FullPath, FullPath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (RootedCanonicalPath)) return false;
            return Equals((RootedCanonicalPath) obj);
        }

        public override int GetHashCode()
        {
            return FullPath.GetHashCode();
        }

        public static bool operator ==(RootedCanonicalPath left, RootedCanonicalPath right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RootedCanonicalPath left, RootedCanonicalPath right)
        {
            return !left.Equals(right);
        }

        #endregion // Value Object


        #region Debugging

        public override string ToString()
        {
            return FullPath;
        }

        #endregion Debugging
    }
}