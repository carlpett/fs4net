﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fs4net.Framework;
using fs4net.Memory.Node;

namespace fs4net.Memory.Impl
{
    internal sealed class MemoryFileSystemImpl : IInternalFileSystem, IDisposable
    {
        // TODO: Make this stuff configurable
        private const string SystemDrive = "c:";
        private const string SpecialFolderRoot = SystemDrive + @"\Users\dude";
        private readonly IDictionary<string, string> _specialFolders = new Dictionary<string, string>
            {
                {"ApplicationData", SpecialFolderRoot + @"\AppData\Roaming"},
                {"CommonApplicationData", @"C:\ProgramData"},
                {"CommonProgramFiles", @"C:\Program Files (x86)\Common Files"},
                {"Cookies", SpecialFolderRoot + @"\AppData\Roaming\Microsoft\Windows\Cookies"},
                {"Desktop", SpecialFolderRoot + @"\Desktop"},
                {"DesktopDirectory", SpecialFolderRoot + @"\Desktop"},
                {"Favorites", SpecialFolderRoot + @"\NetHood\Favorites"},
                {"History", SpecialFolderRoot + @"\AppData\Local\Microsoft\Windows\History"},
                {"InternetCache", SpecialFolderRoot + @"\AppData\Local\Microsoft\Windows\Temporary Internet Files"},
                {"LocalApplicationData", SpecialFolderRoot + @"\AppData\Local"},
                {"MyMusic", SpecialFolderRoot + @"\Music"},
                {"MyPictures", SpecialFolderRoot + @"\Pictures"},
                {"Personal", SpecialFolderRoot + @"\Documents"},
                {"ProgramFiles", @"C:\Program Files (x86)"},
                {"Programs", SpecialFolderRoot + @"\AppData\Roaming\Microsoft\Windows\Start Menu\Programs"},
                {"Recent", SpecialFolderRoot + @"\AppData\Roaming\Microsoft\Windows\Recent"},
                {"SendTo", SpecialFolderRoot + @"\AppData\Roaming\Microsoft\Windows\SendTo"},
                {"StartMenu", SpecialFolderRoot + @"\AppData\Roaming\Microsoft\Windows\Start Menu"},
                {"Startup", SpecialFolderRoot + @"\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup"},
                {"System", @"C:\Windows\system32"},
                {"Templates", SpecialFolderRoot + @"\AppData\Roaming\Microsoft\Windows\Templates"},
                // Temp is not a special folder, but it's stored here to simplify logic (besides, why isn't it a special folder?)
                {"Temp", SpecialFolderRoot + @"\AppData\Local\Temp"},
            };
        private string _currentDirectory;

        private readonly FolderNode _rootNode = FolderNode.CreateRoot();

        public MemoryFileSystemImpl()
        {
            _rootNode.CreateOrReuseFolderNode(SystemDrive);
            foreach (var folder in _specialFolders)
            {
                CreateDirectory(folder.Value);
            }
            _currentDirectory = _specialFolders["Temp"]; // Good default? I could use Directory.GetCurrentDirectory(), but it's not predictable... And it must exist.
        }

        public void WithDrives(string[] driveNames)
        {
            Array.ForEach(driveNames, drive => _rootNode.CreateOrReuseFolderNode(drive));
        }

        public void Dispose()
        {
            _rootNode.Dispose();
        }

        public string GetTemporaryDirectory()
        {
            return _specialFolders["Temp"];
        }

        public string GetSpecialFolder(Environment.SpecialFolder folder)
        {
            var folderKey = folder.ToString();
            if (!_specialFolders.ContainsKey(folderKey)) throw new NotSupportedException(string.Format("{0} cannot be denoted by a RootedDirectory.", folder));
            return _specialFolders[folderKey];
        }

        public IEnumerable<string> AllDrives()
        {
            return _rootNode
                .Children
                .Select(child => child.Name);
        }

        public bool IsFile(RootedCanonicalPath path)
        {
            return (FindFileNodeByPath(path.FullPath) != null);
        }

        public bool IsDirectory(RootedCanonicalPath path)
        {
            return (FindFolderNodeByPath(path.FullPath) != null);
        }

        public long GetFileSize(RootedCanonicalPath path)
        {
            return FindFileNodeByPath(path.FullPath).Size;
        }

        public DateTime GetFileLastWriteTime(RootedCanonicalPath path)
        {
            return FindFileNodeByPath(path.FullPath).LastWriteTime;
        }

        public void SetFileLastWriteTime(RootedCanonicalPath path, DateTime at)
        {
            FindFileNodeByPath(path.FullPath).LastWriteTime = at;
        }

        public DateTime GetDirectoryLastWriteTime(RootedCanonicalPath path)
        {
            return FindFolderNodeByPath(path.FullPath).LastWriteTime;
        }

        public void SetDirectoryLastWriteTime(RootedCanonicalPath path, DateTime at)
        {
            FindFolderNodeByPath(path.FullPath).LastWriteTime = at;
        }

        public DateTime GetFileLastAccessTime(RootedCanonicalPath path)
        {
            return FindFileNodeByPath(path.FullPath).LastAccessTime;
        }

        public void SetFileLastAccessTime(RootedCanonicalPath path, DateTime at)
        {
            FindFileNodeByPath(path.FullPath).LastAccessTime = at;
        }

        public DateTime GetDirectoryLastAccessTime(RootedCanonicalPath path)
        {
            return FindFolderNodeByPath(path.FullPath).LastAccessTime;
        }

        public void SetDirectoryLastAccessTime(RootedCanonicalPath path, DateTime at)
        {
            FindFolderNodeByPath(path.FullPath).LastAccessTime = at;
        }

        public IEnumerable<string> GetFilesInDirectory(RootedCanonicalPath path)
        {
            return FindFolderNodeByPath(path.FullPath)
                .Children
                .OfType<FileNode>()
                .Select(child => child.FullPath);
        }

        public IEnumerable<string> GetDirectoriesInDirectory(RootedCanonicalPath path)
        {
            return FindFolderNodeByPath(path.FullPath)
                .Children
                .OfType<FolderNode>()
                .Select(child => child.FullPath);
        }

        public void CreateDirectory(RootedCanonicalPath path)
        {
            CreateDirectory(path.FullPath);
        }

        public void DeleteFile(RootedCanonicalPath path)
        {
            // TODO: Tolerant if it doesn't exist.
            FindFileNodeByPath(path.FullPath).Delete();
        }

        public void DeleteDirectory(RootedCanonicalPath path, bool recursive)
        {
            // TODO: Tolerant if it doesn't exist; Support recursive.
            FindFolderNodeByPath(path.FullPath).Delete();
        }

        public void MoveFile(RootedCanonicalPath source, RootedCanonicalPath destination)
        {
            var sourceNode = FindFileNodeByPath(source.FullPath);
            MoveFileSystemItem(sourceNode, destination);
        }

        public void MoveDirectory(RootedCanonicalPath source, RootedCanonicalPath destination)
        {
            var sourceNode = FindFolderNodeByPath(source.FullPath);
            MoveFileSystemItem(sourceNode, destination);
        }

        private void MoveFileSystemItem(FileSystemNode sourceNode, RootedCanonicalPath destination)
        {
            var parser = new PathParser(destination.FullPath);
            var destParentNode = parser.GetParentNode(_rootNode);
            string destName = parser.GetLeafNodeName();

            sourceNode.MoveTo(destParentNode, destName);
        }

        public void CopyFile(RootedCanonicalPath source, RootedCanonicalPath destination)
        {
            var sourceNode = FindFileNodeByPath(source.FullPath);
            var parser = new PathParser(destination.FullPath);
            var destParentNode = parser.GetParentNode(_rootNode);
            string destName = parser.GetLeafNodeName();

            sourceNode.CopyTo(destParentNode, destName);
        }

        public void CopyAndOverwriteFile(RootedCanonicalPath source, RootedCanonicalPath destination)
        {
            var sourceNode = FindFileNodeByPath(source.FullPath);
            var parser = new PathParser(destination.FullPath);
            var destParentNode = parser.GetParentNode(_rootNode);
            string destName = parser.GetLeafNodeName();

            sourceNode.CopyTo(destParentNode, destName);
        }

        public Stream CreateReadStream(RootedCanonicalPath path)
        {
            return FindFileNodeByPath(path.FullPath).CreateReadStream();
        }

        public Stream CreateWriteStream(RootedCanonicalPath path)
        {
            return CreateFile(path.FullPath).CreateWriteStream();
        }

        public Stream CreateAppendStream(RootedCanonicalPath path)
        {
            return CreateOrReuseFile(path.FullPath).CreateAppendStream();
        }

        public Stream CreateModifyStream(RootedCanonicalPath path)
        {
            return CreateOrReuseFile(path.FullPath).CreateModifyStream();
        }

        public string GetCurrentDirectory()
        {
            return _currentDirectory;
        }

        public void SetCurrentDirectory(RootedCanonicalPath path)
        {
            _currentDirectory = path.FullPath;
        }


        private FileNode CreateFile(string path)
        {
            return CreateFile(path, (parent, filename) => parent.CreateFileNode(filename));
        }

        private FileNode CreateOrReuseFile(string path)
        {
            return CreateFile(path, (parent, filename) => parent.CreateOrReuseFileNode(filename));
        }

        private FileNode CreateFile(string path, Func<FolderNode, string, FileNode> createStrategy)
        {
            FileNode resultNode = null;
            var currentNode = _rootNode;

            var parser = new PathParser(path);
            parser.WithEachButLastFileSystemNodeNameDo(folderName => currentNode = currentNode.CreateOrReuseFolderNode(folderName));
            parser.WithLastFileSystemNodeNameDo(filename => resultNode = createStrategy(currentNode, filename));
            return resultNode;
        }

        private void CreateDirectory(string path)
        {
            FolderNode currentNode = null;

            var parser = new PathParser(path);
            parser.WithFirstFileSystemNodeNameDo(driveName => currentNode = (FolderNode) _rootNode.FindChildNodeNamed(driveName));
            if (currentNode == null) throw new DirectoryNotFoundException(string.Format("Can't create the directory '{0}' since the drive does not exist.", path));
            parser.WithEachButFirstFileSystemNodeNameDo(folderName => currentNode = currentNode.CreateOrReuseFolderNode(folderName));
        }

        private FolderNode FindFolderNodeByPath(string path)
        {
            return FindNodeByPath(path) as FolderNode;
        }

        private FileNode FindFileNodeByPath(string path)
        {
            return FindNodeByPath(path) as FileNode;
        }

        private FileSystemNode FindNodeByPath(string path)
        {
            FileSystemNode currentNode = _rootNode;

            var parser = new PathParser(path);
            parser.WithEachFileSystemNodeNameDo(delegate(string folderName)
                {
                    if (currentNode != null && currentNode is FolderNode)
                    {
                        var folderNode = (FolderNode)currentNode;
                        currentNode = folderNode.FindChildNodeNamed(folderName);
                    }
                });

            return currentNode;
        }

        public override string ToString()
        {
            return _rootNode.TreeAsString(0);
        }
    }
}