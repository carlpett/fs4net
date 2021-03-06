using System;
using System.Collections.Generic;
using fs4net.Framework.Impl;
using fs4net.Framework.Utils;

namespace fs4net.Framework
{
    /// <summary>
    /// An abstract representation of a file system and can be seen as a factory for rooted path descriptors. The rooted
    /// path descriptors can be used to operate on the file system.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// The IFileSystem interface is a facade that hides implementation details. This property returns an object
        /// representing the internal implementation. In most cases you can ignore this property, but in some testing
        /// scenarios it could be useful.
        /// </summary>
        IInternalFileSystem InternalFileSystem { get; }

        /// <summary>
        /// Returns the logger object where to abnormalities are reported.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Creates a file descriptor from the given path. This method throws if the path is invalid.
        /// This method will succeed even if the file does not exist.
        /// </summary>
        /// <exception cref="System.IO.PathTooLongException">The specified path, in its canonical form, exceeds
        /// the system-defined maximum length.</exception>
        /// <exception cref="fs4net.Framework.InvalidPathException">The specified path contains invalid characters,
        /// contains an invalid drive letter, or is invalid in some other way.</exception>
        /// <exception cref="System.ArgumentNullException">The specified path is null.</exception>
        /// <exception cref="fs4net.Framework.NonRootedPathException">The specified path is relative or empty.</exception>
        RootedFile FileDescribing(string fullPath);

        /// <summary>
        /// Creates a descriptor to a directory from the given path. This method throws if the path is invalid. The
        /// path may not end with a backslash.
        /// This method will succeed even if the directory does not exist.
        /// </summary>
        /// <exception cref="System.IO.PathTooLongException">The specified path, in its canonical form, exceeds
        /// the system-defined maximum length.</exception>
        /// <exception cref="fs4net.Framework.InvalidPathException">The specified path contains invalid characters,
        /// contains an invalid drive letter, or is invalid in some other way.</exception>
        /// <exception cref="System.ArgumentNullException">The specified path is null.</exception>
        /// <exception cref="fs4net.Framework.NonRootedPathException">The specified path is relative or empty.</exception>
        RootedDirectory DirectoryDescribing(string fullPath);

        /// <summary>
        /// Creates a descriptor to a drive from the given drive name. The drive should be given without an ending
        /// backslash. Examples: "c:", "\\network\share".
        /// This method will succeed even if the drive does not exist.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The specified path is null.</exception>
        /// <exception cref="fs4net.Framework.InvalidPathException">The specified path is empty or contains an invalid drive letter.</exception>
        Drive DriveDescribing(string driveName);

        /// <summary>
        /// Creates a file descriptor from the given path. If the given path is relative, the current directory
        /// is used to make the descriptor rooted.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The specified file system or the specified path is null.</exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, in its canonical form, exceeds
        /// the system-defined maximum length.</exception>
        /// <exception cref="fs4net.Framework.InvalidPathException">The specified path contains invalid characters,
        /// contains an invalid drive letter, or is invalid in some other way.</exception>
        RootedFile FileFromCurrentDirectory(string path);

        /// <summary>
        /// Creates a descriptor to a directory from the given path. If the given path is relative, the current
        /// directory is used to make the descriptor rooted.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The specified file system or the specified path is null.</exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, in its canonical form, exceeds
        /// the system-defined maximum length.</exception>
        /// <exception cref="fs4net.Framework.InvalidPathException">The specified path contains invalid characters,
        /// contains an invalid drive letter, or is invalid in some other way.</exception>
        RootedDirectory DirectoryFromCurrentDirectory(string path);

        /// <summary>
        /// Creates a descriptor to the temporary directory.
        /// </summary>
        RootedDirectory DirectoryDescribingTemporaryDirectory();

        /// <summary>
        /// Creates a descriptor to the current current working directory of the application.
        /// </summary>
        RootedDirectory DirectoryDescribingCurrentDirectory();

        /// <summary>
        /// Retrieves descriptors to all logical drives on the computer.
        /// </summary>
        IEnumerable<Drive> AllDrives();
    }

    public static class FileSystemExtensions
    {
        /// <summary>
        /// Creates a file descriptor from the given path. This method throws if the path is invalid.
        /// This method will succeed even if the file does not exist.
        /// </summary>
        /// <exception cref="System.IO.PathTooLongException">The specified path, in its canonical form, exceeds
        /// the system-defined maximum length.</exception>
        /// <exception cref="fs4net.Framework.InvalidPathException">The specified path contains invalid characters,
        /// contains an invalid drive letter, or is invalid in some other way.</exception>
        /// <exception cref="System.ArgumentNullException">The specified path is null.</exception>
        /// <exception cref="fs4net.Framework.NonRootedPathException">The specified path is relative or empty.</exception>
        public static RootedFile FileDescribing(IFileSystem fileSystem, string fullPath)
        {
            ThrowHelper.ThrowIfNull(fileSystem, "fileSystem");
            // TODO: If relative, append it to Current Directory. Or not...?
            return new RootedFile(fileSystem.InternalFileSystem, fullPath, fileSystem.Logger);
        }

        /// <summary>
        /// Creates a descriptor to a directory from the given path. This method throws if the path is invalid. The
        /// path may not end with a backslash.
        /// This method will succeed even if the directory does not exist.
        /// </summary>
        /// <exception cref="System.IO.PathTooLongException">The specified path, in its canonical form, exceeds
        /// the system-defined maximum length.</exception>
        /// <exception cref="fs4net.Framework.InvalidPathException">The specified path contains invalid characters,
        /// contains an invalid drive letter, or is invalid in some other way.</exception>
        /// <exception cref="System.ArgumentNullException">The specified path is null.</exception>
        /// <exception cref="fs4net.Framework.NonRootedPathException">The specified path is relative or empty.</exception>
        public static RootedDirectory DirectoryDescribing(IFileSystem fileSystem, string fullPath)
        {
            ThrowHelper.ThrowIfNull(fileSystem, "fileSystem");
            // TODO: If relative, append it to Current Directory. Or not...?
            return new RootedDirectory(fileSystem.InternalFileSystem, fullPath, fileSystem.Logger);
        }

        /// <summary>
        /// Creates a descriptor to a drive from the given drive name. The drive should be given without an ending
        /// backslash. Examples: "c:", "\\network\share".
        /// This method will succeed even if the drive does not exist.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The specified path is null.</exception>
        /// <exception cref="fs4net.Framework.InvalidPathException">The specified path is empty or contains an invalid drive letter.</exception>
        public static Drive DriveDescribing(IFileSystem fileSystem, string driveName)
        {
            ThrowHelper.ThrowIfNull(fileSystem, "fileSystem");
            return new Drive(fileSystem.InternalFileSystem, driveName, fileSystem.Logger);
        }

        /// <summary>
        /// Creates a file descriptor from the given path. If the given path is relative, the current directory
        /// is used to make the descriptor rooted.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The specified file system or the specified path is null.</exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, in its canonical form, exceeds
        /// the system-defined maximum length.</exception>
        /// <exception cref="fs4net.Framework.InvalidPathException">The specified path contains invalid characters,
        /// contains an invalid drive letter, or is invalid in some other way.</exception>
        public static RootedFile FileFromCurrentDirectory(IFileSystem fileSystem, string path)
        {
            ThrowHelper.ThrowIfNull(fileSystem, "fileSystem");
            if (path.IsValidRootedFile())
            {
                return fileSystem.FileDescribing(path);
            }
            return fileSystem.DirectoryDescribingCurrentDirectory() + RelativeFile.FromString(path);
        }

        /// <summary>
        /// Creates a descriptor to a directory from the given path. If the given path is relative, the current
        /// directory is used to make the descriptor rooted.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The specified file system or the specified path is null.</exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, in its canonical form, exceeds
        /// the system-defined maximum length.</exception>
        /// <exception cref="fs4net.Framework.InvalidPathException">The specified path contains invalid characters,
        /// contains an invalid drive letter, or is invalid in some other way.</exception>
        public static RootedDirectory DirectoryFromCurrentDirectory(this IFileSystem fileSystem, string path)
        {
            ThrowHelper.ThrowIfNull(fileSystem, "fileSystem");
            if (path.IsValidRootedFile())
            {
                return fileSystem.DirectoryDescribing(path);
            }
            return fileSystem.DirectoryDescribingCurrentDirectory() + RelativeDirectory.FromString(path);
        }

        /// <summary>
        /// Creates a descriptor to the special folder identified by the parameter.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The specified file system or the specified path is null.</exception>
        /// <exception cref="fs4net.Framework.InvalidPathException">The path associated with the specified special folder
        /// contains invalid characters, contains an invalid drive letter, or is invalid in some other way.</exception>
        public static RootedDirectory DirectoryDescribingSpecialFolder(this IFileSystem fileSystem, Environment.SpecialFolder folder)
        {
            return fileSystem.DirectoryDescribing(Environment.GetFolderPath(folder));
        }
    }
}
