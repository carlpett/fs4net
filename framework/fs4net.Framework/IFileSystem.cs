﻿namespace fs4net.Framework
{
    public interface IFileSystem
    {
        /// <summary>
        /// Creates a file descriptor from the given path. If the given path is relative, the current directory
        /// is used to make the descriptor rooted.
        /// This method throws if the path is invalid.
        /// </summary>
        /// <exception cref="System.IO.PathTooLongException">The specified path in its canonical form exceeds
        /// the system-defined maximum length.</exception>
        /// <exception cref="System.ArgumentException">The specified path is empty, start or ends with white space,
        /// contains one or more invalid characters or contains an invalid drive letter.</exception>
        /// TODO: The exception list is wrong!
        RootedFile CreateFileDescribing(string fullPath);

        /// <summary>
        /// Creates a descriptor to a directory from the given path. If the given path is relative, the current
        /// directory is used to make the descriptor rooted.
        /// This method throws if the path is invalid.
        /// <exception cref="System.IO.PathTooLongException">The specified path in its canonical form exceeds
        /// the system-defined maximum length.</exception>
        /// <exception cref="System.ArgumentException">The specified path is empty, start or ends with white space,
        /// contains one or more invalid characters or contains an invalid drive letter.</exception>
        /// </summary>
        /// TODO: The exception list is wrong!
        RootedDirectory CreateDirectoryDescribing(string fullPath);

        /// <summary>
        /// Creates a descriptor to the temporary directory.
        /// </summary>
        RootedDirectory CreateDirectoryDescribingTemporaryDirectory();

        /// <summary>
        /// Creates a descriptor to the current directory.
        /// </summary>
        RootedDirectory CreateDirectoryDescribingCurrentDirectory(); // TODO: There's one per drive, right?

        // TODO:
        //  Descriptors based on Environment.SpecialFolder enumeration

        /// <summary>
        /// Creates a descriptor to a drive from the given drive name. The drive should be given without an ending
        /// backslash. Examples: "c:", "\\network\share".
        /// </summary>
        /// TODO: Exception list!
        Drive CreateDriveDescribing(string driveName);
    }
}