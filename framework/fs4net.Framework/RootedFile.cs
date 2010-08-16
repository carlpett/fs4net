using System;
using System.IO;
using fs4net.Framework.Impl;

namespace fs4net.Framework
{
    /// <summary>
    /// Represents a the full path to a file. The path is rooted, meaning that it starts with a drive (e.g. c:, d:,
    /// \\network\drive, etc).
    /// </summary>
    public class RootedFile : IFile<RootedFile>, IRootedFileSystemItem<RootedFile>
    {
        private readonly IInternalFileSystem _fileSystem;
        private readonly string _rootedPath;
        private readonly Func<string, string> _pathWasher;
        private readonly string _canonicalFullPath;

        public RootedFile(IInternalFileSystem fileSystem, string rootedPath, Func<string, string> pathWasher) // TODO: public only for unit tests... necessary? Use InternalsVisibleTo attribute?
        {
            ThrowHelper.ThrowIfNull(fileSystem, "fileSystem");
            _fileSystem = fileSystem;
            _rootedPath = pathWasher(rootedPath);
            _pathWasher = pathWasher;
            _canonicalFullPath = new CanonicalPathBuilder(_rootedPath).BuildForRootedFile();
        }

        #region Interface Implementations

        /// <summary>
        /// Returns the FileSystem with which this descriptor is associated.
        /// </summary>
        public IFileSystem FileSystem
        {
            get { return _fileSystem; }
        }

        /// <summary>
        /// Returns the path that this descriptor represent as a string. It is returned on the same format as it was
        /// created with. This means that it can contain redundant parts such as ".", ".." inside paths and multiple
        /// "\". To remove such redundant parts, use the AsCanonical() factory method.
        /// This property succeeds whether the file exists or not.
        /// </summary>
        public string PathAsString
        {
            get { return _rootedPath; }
        }

        public Func<string, string> PathWasher
        {
            get { return _pathWasher; }
        }

        /// <summary>
        /// Returns a descriptor where the PathAsString property returns the path on canonical form. A canonical
        /// descriptor does not contain any redundant names, which means that ".", ".." and extra "\" have been removed
        /// from the string that the descriptor was created with.
        /// This method succeeds whether the file exists or not.
        /// </summary>
        public RootedFile AsCanonical() // TODO? Make into extension method and add a Clone() method?
        {
            return new RootedFile(_fileSystem, _canonicalFullPath, PathWasher);
        }

        #endregion // Interface Implementations

        #region Public Interface

        /// <summary>
        /// Returns a descriptor where the two descriptors are concatenated.
        /// </summary>
        //public static RootedFile operator +(RelativeDirectory lhs, RelativeFile rhs)
        //{
        //    return new RelativeFile(Path.Combine(lhs.PathAsString, rhs.PathAsString));
        //}

        // TODO: More stuff:
        //  * Parent folder


        #endregion // Public Interface

        #region // Implementation Details

        #endregion // Implementation Details


        #region Override Object

        // TODO: What to do with these...? Extension methods? Skip and have those with better names?
        public bool Equals(RootedFile other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._fileSystem, _fileSystem) && Equals(other._rootedPath, _rootedPath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(RootedFile)) return false;
            return Equals((RootedFile)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_fileSystem.GetHashCode() * 397) ^ _rootedPath.GetHashCode();
            }
        }

        #endregion // Override Object
    }

    public static class RootedFileExtensions
    {
        /// <summary>
        /// Tests whether the file exists. Returns true if a file with the given name exists. If a directory with the
        /// given name exists it returns false.
        /// </summary>
        public static bool Exists(this RootedFile me)
        {
            return me.IsFile();
        }

        /// <summary>
        /// Returns the date and time the file was last written to.
        /// </summary>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission</exception>
        /// <exception cref="System.IO.FileNotFoundException">If the file does not exist.</exception>
        public static DateTime LastModified(this RootedFile me)
        {
            if (me.IsFile() == false)
            {
                if (me.IsDirectory())
                {
                    throw new FileNotFoundException(string.Format("Can't get last modified time for file '{0}' since it denotes a directory.", me.PathAsString));
                }
                throw new FileNotFoundException(string.Format("Can't get last modified time for file '{0}' since it does not exist.", me.PathAsString));
            }
            return me.InternalFileSystem().GetFileLastModified(me.CanonicalPathAsString());
        }

        /// <summary>
        /// Deletes the file denoted by this descriptor. If the file does not exists this method does nothing.
        /// </summary>
        /// <exception cref="System.IO.IOException">The file is in use; A directory is denoted by this file
        /// descriptor.</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission;
        /// The descriptor denotes a read-only file</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it
        /// is on an unmapped drive).</exception>
        public static void Delete(this RootedFile me)
        {
            if (me.IsDirectory())
            {
                throw new IOException(string.Format("Can't delete the file '{0}' since it denotes a directory.", me.PathAsString));
            }
            if (me.Exists())
            {
                var fileSystem = me.InternalFileSystem();
                var path = me.CanonicalPathAsString();
                //if (fileSystem.IsFileInUse(path))
                //{
                //    // TODO: Better/more specifiec exception?
                //    throw new IOException(string.Format("Can't delete the file '{0}' since it's in use.", me.PathAsString));
                //}
                //// Attributes: Archive, ReadOnly, Hidden, System, Device, ...
                //if (fileSystem.GetAttributes(path) == FileAttributes.ReadOnly)
                //{
                //    throw new UnauthorizedAccessException(string.Format("Can't delete the read-only file '{0}'.", me.PathAsString));
                //}
                //if (fileSystem.IsReady(me.DriveName()) == false)
                //{
                //    throw new DirectoryNotFoundException(string.Format("Can't delete the file '{0}' since the drive is not ready.", me.PathAsString));
                //}
                fileSystem.DeleteFile(path);
            }
        }

        /// <summary>
        /// Tries to deletes the file denoted by this descriptor.
        /// </summary>
        /// <exception cref="System.IO.PathTooLongException">If the file descriptor is relative and concatenated
        /// with the current directory it exceeds the system-defined maximum length.</exception>
        /// <returns>
        /// True if the file no longer exists. That is, the file was either deleted, or it
        /// did not exist to start with. If the file descriptor denotes a directory this method
        /// returns true.
        /// </returns>
        public static bool TryDelete(this RootedFile me)
        {
            if (me.Exists())
            {
                var fileSystem = me.InternalFileSystem();
                var path = me.CanonicalPathAsString();
                //if (fileSystem.IsFileInUse(path))
                //{
                //    return false;
                //}
                //if (fileSystem.GetAttributes(path) == FileAttributes.ReadOnly)
                //{
                //    return false;
                //}
                //if (fileSystem.IsReady(me.DriveName()))
                //{
                //    return false;
                //}
                try
                {
                    fileSystem.DeleteFile(path);
                }
// ReSharper disable EmptyGeneralCatchClause
                catch { }
// ReSharper restore EmptyGeneralCatchClause
            }
            return me.Exists();
        }

        /// <summary>
        /// Opens a read stream with the file denoted by this file descriptor as source.
        /// </summary>
        /// <exception cref="System.IO.FileNotFoundException">The file cannot be found</exception>
        /// <exception cref="System.IO.PathTooLongException">If the file descriptor is relative and concatenated
        /// with the current directory it exceeds the system-defined maximum length.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it
        /// is on an unmapped drive).</exception>
        /// <returns></returns>
        public static Stream CreateReadStream(this RootedFile me)
        {
            if (me.IsDirectory())
            {
                // TODO: Better/more specifiec exception?
                throw new IOException(string.Format("Can't open a file '{0}' for reading since the path denotes a directory.", me.PathAsString));
            }
            if (me.IsFile() == false)
            {
                throw new FileNotFoundException(string.Format("Can't open the file '{0}' for reading since it does not exists.", me.PathAsString));
            }
            return me.InternalFileSystem().CreateReadStream(me.CanonicalPathAsString());
        }

        /// <summary>
        /// Opens a write stream with the file denoted by this file descriptor as source. If the file already exists
        /// the file is overwritten.
        /// </summary>
        /// <exception cref="System.IO.PathTooLongException">If the file descriptor is relative and concatenated
        /// with the current directory it exceeds the system-defined maximum length.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid.</exception>
        /// <returns></returns>
        public static Stream CreateWriteStream(this RootedFile me)
        {
            if (me.IsDirectory())
            {
                // TODO: Better/more specifiec exception?
                throw new IOException(string.Format("Can't create the file '{0}' since the path denotes an existing directory.", me.PathAsString));
            }
            // TODO: Check if parent directory exists
            return me.InternalFileSystem().CreateWriteStream(me.CanonicalPathAsString());
        }
    }
}