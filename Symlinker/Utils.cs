using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SymbolicLinkSupport;

namespace Symlinker
{
    public static class Utils
    {
        public static void CreateSymlink(this SymlinkFolderData symlinkFolderData)
        {
            foreach (var targetFolder in symlinkFolderData.TargetFolders)
            {
                if (Directory.Exists(targetFolder))
                {
                    var file = new DirectoryInfo(targetFolder);
                    if (!file.IsSymbolicLink() || !file.IsSymbolicLinkValid())
                    {
                        Directory.Delete(targetFolder);
                        CreateSymlinkInternal();
                    }
                }
                else
                {
                    CreateSymlinkInternal();
                }


                void CreateSymlinkInternal()
                {
                    symlinkFolderData.DirectoryInfo.CreateSymbolicLink(targetFolder, !Path.IsPathRooted(targetFolder));
                    
                                    
                    foreach (var folder in  symlinkFolderData.TargetFolders)
                    {
                        Console.WriteLine(symlinkFolderData.DirectoryInfo.FullName + "--->" + folder);
                    }
                }
            }
        }

        public static void CreateSymlink(this SymlinkFileData symlinkFileData)
        {
            foreach (var targetFileName in symlinkFileData.TargetFileNames)
            {
                if (File.Exists(targetFileName))
                {
                    var file = new FileInfo(targetFileName);
                    if (!file.IsSymbolicLink() || !file.IsSymbolicLinkValid())
                    {
                        File.Delete(targetFileName);
                        CreateSymlinkInternal();
                    }
                }
                else
                {
                    CreateSymlinkInternal();
                }


                void CreateSymlinkInternal()
                {
                    symlinkFileData.FileInfo.CreateSymbolicLink(targetFileName, !Path.IsPathRooted(targetFileName));
                    
                    foreach (var folder in  symlinkFileData.TargetFileNames)
                    {
                        Console.WriteLine(symlinkFileData.FileInfo.FullName + "--->" + folder);
                    }
                }
                
            }
        }


        public static FileInfo GetFileInfo(string fileName)
        {
            return new FileInfo(fileName);
        }

        public static DirectoryInfo GetDirectoryInfo(string fileName)
        {
            return new DirectoryInfo(fileName);
        }

        public static void GetFilesAndDirectories(string directory, out DirectoryInfo[] dirs, out FileInfo[] files)
        {
            var dirInfo = new DirectoryInfo(directory);
            if (!dirInfo.Exists) throw new IOException("插件文件夹不存在");
            dirs = dirInfo.GetDirectories();
            files = dirInfo.GetFiles();
        }

        public static void GetFilesAndDirectories(IEnumerable<string> directories, out List<FileInfo> fileInfos,
            out List<DirectoryInfo> dirInfos)
        {
            fileInfos = new List<FileInfo>();
            dirInfos = new List<DirectoryInfo>();

            foreach (var directory in directories)
            {
                var dirsData = new DirectoryInfo[] { };
                var filesData = new FileInfo[] { };
                GetFilesAndDirectories(directory, out dirsData, out filesData);
                fileInfos.AddRange(filesData);
                dirInfos.AddRange(dirsData);
            }
        }
    }
}