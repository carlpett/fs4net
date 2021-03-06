using fs4net.Framework;

namespace fs4net.Builder
{
    public sealed class FileSystemBuilder
    {
        private readonly IFileSystem _fileSystem;
        private readonly RootedDirectory _rootDir;

        public FileSystemBuilder(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _rootDir = null;
        }

        public FileSystemBuilder(IFileSystem fileSystem, RootedDirectory rootDir)
        {
            _fileSystem = fileSystem;
            _rootDir = rootDir;
        }

        public RootedDirectoryBuilder WithDir(string path)
        {
            return _rootDir == null
                ? new RootedDirectoryBuilder(_fileSystem.DirectoryDescribing(path))
                : new RootedDirectoryBuilder(_rootDir + RelativeDirectory.FromString(path));
        }

        public RootedFileBuilder WithFile(string path)
        {
            return _rootDir == null
                ? new RootedFileBuilder(_fileSystem.FileDescribing(path))
                : new RootedFileBuilder(_rootDir + RelativeFile.FromString(path));
        }
    }
}